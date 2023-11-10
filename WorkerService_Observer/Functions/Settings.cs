using Lib.CommonFunctions;
using Lib.CommonFunctions.Interfaces;
using WorkerService_Observer.Core;

namespace WorkerService_Observer.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerObserver> _logger;

        private readonly ICommonFunctions commonFunctions;

        public Settings(ILogger<WorkerObserver> logger) 
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

            // #3
            int lResult;

            string? lValue = config["MaxCountOfProceedProcesses"];

            if (!int.TryParse(lValue, out lResult))
            {
                lResult = 5;
            }

            AppData.MaxCountOfProceedProcesses = lResult;
        }
    }
}
