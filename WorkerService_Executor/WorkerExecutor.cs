using Lib.DataTypes;
using Lib.DataTypes.EF;
using WorkerService_Executor.EF;
using WorkerService_Executor.Functions;
using WorkerService_Executor.Interfaces;

namespace WorkerService_Executor
{
    public class WorkerExecutor : BackgroundService, IWorkerExecutor, IDisposable
    {
        private readonly ILogger<WorkerExecutor> _logger;
        private ParseHelper parseHelper;

        private readonly AppDbContext appDbContext;

        public WorkerExecutor(ILogger<WorkerExecutor> logger)
        {
            _logger = logger;

            appDbContext = new AppDbContext();

            parseHelper = new ParseHelper(_logger, appDbContext);

            // Load settings
            new Settings(_logger).ProceedConfigFile();

        }

        public override void Dispose()
        {
            appDbContext.Dispose();

            base.Dispose();
        }

        public async void StartWithParametersAsync(Message aMessage, CancellationToken cancellationToken)
        {
            //
            _logger.LogInformation($"WorkerExecutor started with parameters: FileName {aMessage.FileName}. Id {aMessage.TrackFileId}");

            _logger.LogInformation($"Start with file \"{aMessage.FileName}\" at {DateTime.Now}");

            // Run
            DataFileResult? dataFileResult = null;

            // Update db -- Update Start Proceed Time
            // After first usage we we get Id
            TrackLog_Files? file;
            file = appDbContext.TrackLog_Files.Where(x => x.TrackFileId == aMessage.TrackFileId && x.FileStartProceedTime == null).FirstOrDefault();

            if (file == null) 
            {
                _logger.LogError($"Entry not found in TrackLog_Files table! File name \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. 1st update");
                return;
            }

            file.FileStartProceedTime = DateTime.Now;

            appDbContext.SaveChanges();

            // Proceed file
            try
            {
                dataFileResult = await parseHelper.ProceedDataFile(aMessage.FileName, aMessage.TrackFileId);
            }
            finally
            {
                if (dataFileResult != null)
                {
                    // Update db
                    // Update # of Files OK
                    // Update # of Files Failed
                    // Update overall status

                    // Update End Proceed Time
                    // Ok, now we know Id and can use it
                    file = appDbContext.TrackLog_Files.Where(x => x.TrackFileId == file.TrackFileId && x.FileStartProceedTime != null && x.FileFinishProceedTime == null).FirstOrDefault();

                    if (file == null)
                    {
                        _logger.LogError($"Entry not found in TrackLog_Files table! File name \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. 2nd update");
                    }
                    else
                    {
                        file.ErrorMessage = dataFileResult.ErrorMessaget;
                        file.EntriesInFilesOK = dataFileResult.EntriesInFilesOK;
                        file.EntriesInFilesFailed = dataFileResult.EntriesInFilesFailed;
                        file.OverallSuccessStatus = dataFileResult.Suceeded;

                        file.FileFinishProceedTime = DateTime.Now;
                        appDbContext.SaveChanges();
                    }
                }
            }

            // End
            _logger.LogInformation($"Ended with file \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. Proceeded records: {dataFileResult.EntriesInFilesOK}. Failed records: {dataFileResult.EntriesInFilesFailed}. Overall status: {dataFileResult.Suceeded}");
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