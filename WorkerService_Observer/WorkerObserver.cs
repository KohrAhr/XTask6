using Lib.DataTypes;
using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using WorkerService_Observer.Core;
using WorkerService_Observer.Functions;

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

            // Set up watchers
            SetupWatchers("E:\\My\\SDK\\XTask5\\data_in\\s1", "*.txt");

            _logger.LogInformation("Watcher is started!");

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

            FileSystemWatcher fileWatcher = new FileSystemWatcher
            {
                Path = aPath,
                Filter = aFileMask,
                EnableRaisingEvents = true
            };
            fileWatcher.Created += OnFileCreated;

            result++;

            return result;
        }

        /// <summary>
        ///     New file is available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Ok, take the file.
            string aFile = e.FullPath;

            // Make sure its not proceeded yet
            if (string.IsNullOrEmpty(aFile))
            {
                _logger.LogError("File name is empty");
                return;
            }

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