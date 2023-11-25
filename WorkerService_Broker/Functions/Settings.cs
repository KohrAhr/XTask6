using Lib.CommonFunctions.Interfaces;
using WorkerService_Broker.Core;

namespace WorkerService_Broker.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerBroker> _logger;
        private readonly ICommonFunctions _commonFunctions;

        public Settings(ILogger<WorkerBroker> logger, ICommonFunctions aCommonFunctions) 
        {
            _logger = logger;

            _commonFunctions = aCommonFunctions;
            _commonFunctions.SetLogger(_logger);
        }

        public void ProceedConfigFile()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            // #1
            AppData.QueuePath = _commonFunctions.ReadCriticalParameter(config, "QueuePath");

            // #2
            AppData.QueueServer = _commonFunctions.ReadCriticalParameter(config, "QueueServer");
        }
    }
}
