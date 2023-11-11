using Lib.CommonFunctions;
using Lib.CommonFunctions.Interfaces;
using WorkerService_Executor.Core;

namespace WorkerService_Executor.Functions
{
    public class Settings
    {
        private readonly ILogger<WorkerExecutor> _logger;
        private readonly ICommonFunctions commonFunctions;

        public Settings(ILogger<WorkerExecutor> logger) 
        {
            _logger = logger;

            commonFunctions = new CommonFunctions(_logger);
        }

        public void ProceedConfigFile()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            // #1
            AppData.ConnectionString = commonFunctions.ReadCriticalParameter(config, "ConnectionString");

            // #2
            AppData.FileMaxAccessWait = commonFunctions.ReadIntParameter(config, "FileMaxAccessWait", AppData.CONST_DEF_FileMaxAccessWait);
            AppData.SleepBetweenFileAccessAttempt = commonFunctions.ReadIntParameter(config, "SleepBetweenFileAccessAttempt", AppData.CONST_DEF_SleepBetweenFileAccessAttempt);
        }
    }
}
