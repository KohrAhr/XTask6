using Lib.CommonFunctions.Interfaces;
using WorkerService_Observer.Core;

namespace WorkerService_Observer.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerObserver> _logger;

        private readonly ICommonFunctions _commonFunctions;

        public Settings(ILogger<WorkerObserver> logger, ICommonFunctions aCommonFunctions) 
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

            AppData.ConnectionString = _commonFunctions.ReadCriticalParameter(config, "ConnectionString");

            AppData.ScopeOfFolders = _commonFunctions.ReadIntParameter(config, "ScopeOfFolders");

            AppData.EventRecycler = _commonFunctions.ReadIntParameter(config, "EventRecycler", 30);
        }
    }
}
