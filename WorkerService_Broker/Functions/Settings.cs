using Lib.CommonFunctions;
using Lib.CommonFunctions.Interfaces;
using WorkerService_Broker.Core;

namespace WorkerService_Broker.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerBroker> _logger;
        private readonly ICommonFunctions commonFunctions;

        public Settings(ILogger<WorkerBroker> logger) 
        {
            _logger = logger;

            commonFunctions = new CommonFunctions(_logger);
        }

        public void ProceedConfigFile()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            // #1
            AppData.QueuePath = commonFunctions.ReadCriticalParameter(config, "QueuePath");

            // #2
            AppData.QueueServer = commonFunctions.ReadCriticalParameter(config, "QueueServer");
        }
    }
}
