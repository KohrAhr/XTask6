using Lib.CommonFunctions.Interfaces;
using WorkerService_Executor.Core;

namespace WorkerService_Executor.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerExecutor> _logger;
        private readonly ICommonFunctions _commonFunctions;

        public Settings(ILogger<WorkerExecutor> logger, ICommonFunctions aCommonFunctions) 
        {
            _logger = logger;

            _commonFunctions = aCommonFunctions;
            _commonFunctions.SetLogger(_logger);
        }

        public void ProceedConfigFile()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            // #1
            AppData.ConnectionString = _commonFunctions.ReadCriticalParameter(config, "ConnectionString");

            // #2
            AppData.FileMaxAccessWait = _commonFunctions.ReadIntParameter(config, "FileMaxAccessWait", AppData.CONST_DEF_FileMaxAccessWait);
            AppData.SleepBetweenFileAccessAttempt = _commonFunctions.ReadIntParameter(config, "SleepBetweenFileAccessAttempt", AppData.CONST_DEF_SleepBetweenFileAccessAttempt);
        }
    }
}
