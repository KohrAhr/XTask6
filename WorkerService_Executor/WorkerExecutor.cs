using Lib.DataTypes;
using WorkerService_Executor.Functions;
using WorkerService_Executor.Interfaces;

namespace WorkerService_Executor
{
    public class WorkerExecutor : BackgroundService, IWorkerExecutor
    {
        private readonly ILogger<WorkerExecutor> _logger;
        private ParseHelper parseHelper;

        public WorkerExecutor(ILogger<WorkerExecutor> logger)
        {
            _logger = logger;

            parseHelper = new ParseHelper(_logger);
        }

        public void StartWithParametersAsync(string aFileName, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"WorkerExecutor started with parameters: aFileName={aFileName}");

            _logger.LogInformation($"Start with file \"{aFileName}\" at {DateTime.Now}");

            // Update db
            //  Update Start Proceed Time

            // Proceed file
            DataFileResult dataFileResult = parseHelper.ProceedDataFile(aFileName);

            // Update db
                // Update # of Files OK
                // Update # of Files Failed
                // Update overall status

                // Update End Proceed Time

            _logger.LogInformation($"Ended with file \"{aFileName}\". Proceeded records: {dataFileResult.EntriesInFilesOK}. Failed records: {dataFileResult.EntriesInFilesFailed}. Overall status: {dataFileResult.Suceeded}");
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