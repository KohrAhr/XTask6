using Lib.DataTypes;
using Lib.RabbitMQ.Interfaces;
using RabbitMQ.Client;
using WorkerService_Observer.Core;
using WorkerService_Observer.Functions;
using Lib.DataTypes.EF;
using System.Text.Json;
using Lib.AppDb.Interfaces;
using Lib.CommonFunctions;

namespace WorkerService_Observer
{
    public class WorkerObserver : BackgroundService
    {
        private readonly ILogger<WorkerObserver> _logger;
        private readonly IAppDbContext _appDbContext;
        private IRabbitMQHelper _rabbitMQHelper;

        /// <summary>
        ///     List of folders we are observe
        /// </summary>
        private List<FileSystemWatcher> fileSystemWatchers = new List<FileSystemWatcher>();

        #region RabbitMQ
        private ConnectionFactory? factory = null;
        private IConnection? connection = null;
        private IModel? channel = null;
        #endregion RabbitMQ

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        public WorkerObserver(ILogger<WorkerObserver> logger, IRabbitMQHelper aRabbitMQHelper, IAppDbContext aAppDbContext)
        {
            _logger = logger;

            _appDbContext = aAppDbContext;

            _rabbitMQHelper = aRabbitMQHelper;
            _rabbitMQHelper.SetLogger(_logger);
       }

        /// <summary>
        ///     Entry
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Load settings
            new Settings(_logger, new CommonFunctions()).ProceedConfigFile();

            // Only once settings has been loaded.
            _appDbContext.SetConnectionString(AppData.ConnectionString);

            // Init RabbitMQ pipeline
            try
            {
                _rabbitMQHelper.InitRabbitMQ(AppData.QueueServer, AppData.QueuePath, out factory, out connection, out channel);
            }
            catch (Exception ex)
            {
                // Problems with MQ Server
                _logger.LogError("Problems with MQ Server. Error is {ex.Message}", ex.Message);

                Environment.Exit(3);
            }

            // Load settings from db
            _logger.LogInformation(
                "Watcher is initializing for Scope Of with Folders with Id {AppData.ScopeOfFolders}. MQ Server is {AppData.QueueServer}. Pipeline is {AppData.QueuePath}. " +
                "Event Recycler, helpful when one instance handling multiply folders, set to {AppData.EventRecycler} seconds",
                AppData.ScopeOfFolders, AppData.QueueServer, AppData.QueuePath, AppData.EventRecycler
            );

            IList<Config_Folders>? folders = null;
            try
            {
                folders = _appDbContext.Config_Folders.Where(x => x.FolderIsActive == true && x.AssignToObserver == AppData.ScopeOfFolders).ToList();
            }
            catch (Exception ex)
            {
                // Problems with Db?
                // TODO:
                _logger.LogError($"Cannot run DDL query. Error message is {ex.Message}", ex.Message);

                Environment.Exit(2);
            }

            int result = 0;
            // Set up all watchers
            foreach (Config_Folders folder in folders) 
            {
                result += SetupWatchers(folder.FolderToObserver, folder.FilePattern);
            }

            if (result == 0) 
            {
                _logger.LogInformation("No folders to observe!");

                // Return error code 1. 
                // May be not the best way
                Environment.Exit(1);
            }

            _logger.LogInformation("Watcher is started for {result} folder(s)!", result);

            // We are good to continue
            return ExecuteAsync(cancellationToken);
        }



        // If the FileSystemWatcher initially works and then stops capturing events for some folders, a few common reasons could cause this behavior:
        // Buffer Overflow: The buffer of the FileSystemWatcher might overflow if too many events occur within a short period. This could lead to missing subsequent events.
        // Rate of Change: If the rate of file changes in those folders is very high, the FileSystemWatcher might miss events due to the speed at which it processes event
        // Reassigning Event Handlers: Reassigning event handlers or enabling/disabling the FileSystemWatcher frequently could cause it to miss events.

        /// <summary>
        ///     Setup folder watcher
        /// </summary>
        /// <param name="aPath"></param>
        /// <param name="aFileMask"></param>
        /// <returns></returns>
        public int SetupWatchers(string aPath, string aFileMask)
        {
            int result = 0;

            try
            {
                // FileSystemWatcher Limitations:
                // It's worth noting that FileSystemWatcher has limitations and might not capture all events under various circumstances,
                // especially in high-frequency or rapidly changing environments.
                FileSystemWatcher fileWatcher = new FileSystemWatcher
                {
                    Path = aPath,
                    Filter = aFileMask
                };
                fileWatcher.Created += OnFileCreated;
                
                // Enable raising events after(!) setting up the event handlers
                fileWatcher.EnableRaisingEvents = true;

                // !
                fileSystemWatchers.Add(fileWatcher);

                _logger.LogInformation("Watcher for folder \"{aPath}\" created. File mask: \"{aFileMask}\"", aPath, aFileMask);

                result++;
            }
            catch (Exception ex) 
            {
                // Folder does not exist
                // TODO: Return better message
                _logger.LogWarning("Folder \"{aPath}\" does not exist or does not accessible. Error message {ex.Message}", aPath, ex.Message);
            }

            return result;
        }

        /// <summary>
        ///     New file is available. 
        ///     If its not in db yet, then
        ///     1) message to MQ server
        ///     2) new entry to db table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Ok, take the file.
            string aFile = e.FullPath;

            // Check main params
            if (string.IsNullOrEmpty(aFile))
            {
                _logger.LogError("File name is empty");
                return;
            }

            // Make sure its not proceeded yet
            // Should we make assumption that all file names are unique?
            // Need to clarify specification from BA
            int result = _appDbContext.TrackLog_Files.Where(x => x.FileFullPath == aFile).Count();
            if (result > 0)
            {
                // We did this file before?

                _logger.LogWarning("File {aFile} already in system.", aFile);

                // SHould be inform Operator?

                // Bye for now
                return;
            }

            // Setup new MS message
            Message message = new Message
            {
                FileName = aFile
            };

            // Add entry into db table Files
            TrackLog_Files trackLog_Files = new TrackLog_Files
            {
                FileFullPath = aFile
            };

            _appDbContext.TrackLog_Files.Add(trackLog_Files);
            _appDbContext.SaveChanges();

            message.TrackFileId = trackLog_Files.TrackFileId;

            _logger.LogInformation("New incomming file \"{message.FileName}\" at {DateTime.Now} with Id {message.TrackFileId}", message.FileName, DateTime.Now, message.TrackFileId);


            // Send message to MSMQ/RabbitMQ
            string rawMessage = JsonSerializer.Serialize(message);
            _rabbitMQHelper?.SendMessage(channel, AppData.QueuePath, rawMessage);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                // Self repair ? or to overflow ?

                // Recycle events?
                RecycleEvents(fileSystemWatchers);

                try
                {
                    await Task.Delay(AppData.EventRecycler * 1000, stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    // What we can do?
                    _logger.LogError("Exception on Task cancellation. {ex.Message}", ex.Message);
                }
            }
        }

        private void RecycleEvents(List<FileSystemWatcher> aWatchedFolders)
        {
            foreach (FileSystemWatcher watcher in aWatchedFolders)
            {
                watcher.EnableRaisingEvents = false;
                watcher.EnableRaisingEvents = true;
            }
        }
    }
}