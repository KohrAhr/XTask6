using Lib.DataTypes;
using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WorkerService_Observer.Core;
using WorkerService_Observer.EF;
using WorkerService_Observer.Functions;
using WorkerService_Observer.EF.Types;

namespace WorkerService_Observer
{
    public class WorkerObserver : BackgroundService
    {
        private readonly ILogger<WorkerObserver> _logger;

        private IRabbitMQHelper rabbitMQHelper;

        #region RabbitMQ
        private ConnectionFactory? factory = null;
        private IConnection? connection = null;
        private IModel? channel = null;
        #endregion RabbitMQ

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        public WorkerObserver(ILogger<WorkerObserver> logger)
        {
            _logger = logger;

            rabbitMQHelper = new RabbitMQHelper(_logger);
        }

        /// <summary>
        ///     Entry
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Load settings
            new Settings(_logger).ProceedConfigFile();

            // Init RabbitMQ pipeline
            rabbitMQHelper.InitRabbitMQ(AppData.QueueServer, AppData.QueuePath, out factory, out connection, out channel);

            // Load settings from db
            _logger.LogInformation("Watcher is initializing");

            IList<Config_Folders>? folders = null;
            try
            {
                using (AppDbContext context = new AppDbContext())
                {
                    folders = context.Config_Folders.Where(x => x.FolderIsActive == true && x.AssignToObserver == AppData.ScopeOfFolders).ToList();
                }
            }
            catch (Exception ex)
            {
                // Problems with Db?
                // TODO:
                _logger.LogError(ex.Message);

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

            _logger.LogInformation($"Watcher is started for {result} folder(s)!");

            // We are good to continue
            return ExecuteAsync(cancellationToken);
        }

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

                _logger.LogInformation($"Watcher for folder \"{aPath}\" created. File mask: \"{aFileMask}\"");

                result++;
            }
            catch (Exception ex) 
            {
                // Folder does not exist
                // TODO: Return better message
                _logger.LogWarning($"Folder \"{aPath}\" does not exist or does not accessible. Error message {ex.Message}");
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

            // TODO: Make sure its not proceeded yet
            // Should we make assumption that all file names are unique?
            // Need to clarify specification from BA
            using (AppDbContext context = new AppDbContext())
            {
                int result = context.TrackLog_Files.Where(x => x.FileFullPath == aFile).Count();
                if (result > 0)
                {
                    // We did this file before?

                    _logger.LogWarning($"File {aFile} already in system.");

                    // SHould be inform Operator?

                    // Bye for now
                    return;
                }
            }

            // Setup new MS message
            Message message = new Message
            {
                FileName = aFile
            };

            // Send message to MSMQ/RabbitMQ
            string rawMessage = JsonConvert.SerializeObject(message);
            rabbitMQHelper?.SendMessage(channel, AppData.QueuePath, rawMessage);

            //
            _logger.LogInformation($"New incomming file \"{message.FileName}\" at {DateTime.Now}");

            // Add entry into db table Files if succedded with MSMQ
            TrackLog_Files trackLog_Files = new TrackLog_Files();
            trackLog_Files.FileFullPath = aFile;

            using (AppDbContext context = new AppDbContext())
            {
                context.TrackLog_Files.Add(trackLog_Files);
                context.SaveChanges();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
//                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Task.Delay(1000, stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    // What we can do?
                    _logger.LogError($"Exception on Task cancellation. {ex.Message}");
                }
            }
        }
    }
}