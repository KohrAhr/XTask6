using Lib.DataTypes;
using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WorkerService_Broker.Core;
using WorkerService_Broker.Functions;
using WorkerService_Executor.Interfaces;

namespace WorkerService_Broker
{
    public class WorkerBroker : BackgroundService
    {
        private readonly ILogger<WorkerBroker> _logger;
        private readonly IHost _executorHost;

        private readonly IRabbitMQHelper rabbitMQHelper;

        #region RabbitMQ
        private ConnectionFactory? factory = null;
        private IConnection? connection = null;
        private IModel? channel = null;
        #endregion RabbitMQ

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="executorHost"></param>
        public WorkerBroker(ILogger<WorkerBroker> logger, IHost executorHost)
        {
            _logger = logger;

            _executorHost = executorHost;

            rabbitMQHelper = new RabbitMQHelper(logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Load settings
            new Settings(_logger).ProceedConfigFile();

            // Init RabbitMQ pipeline
            rabbitMQHelper.InitRabbitMQ(AppData.QueueServer, AppData.QueuePath, out factory, out connection, out channel);

            // Setup listiner
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(AppData.QueuePath, true, consumer);

            _logger.LogInformation("Broker is ready!");

            // We are good to continue
            return ExecuteAsync(cancellationToken);
        }

        /// <summary>
        ///     Listiner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            string result = rabbitMQHelper.GetMessage(e);

            if (String.IsNullOrEmpty(result)) 
            {
                _logger.LogError($"Broken message received. Data dump: {result}");
                return;
            }

            Message? message = JsonConvert.DeserializeObject<Message>(result);

            if (message == null) 
            {
                _logger.LogError($"Broken message received. Transformation failed. Data dump: {result}");
                return;
            }

            // Run Executor for file
            _logger.LogInformation($"File is available {message.FileName}.");

            // Start the other WorkerService.
            IWorkerExecutor otherWorker = _executorHost.Services.GetRequiredService<IWorkerExecutor>();
            _ = Task.Run(
                () => otherWorker.StartWithParametersAsync(message.FileName, new CancellationToken())
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
//                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await Task.Delay(AppData.DelayInSeconds * 1000, stoppingToken);
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