using Lib.DataTypes;
using Lib.DataTypes.EF;
using Lib.AppDb.Interfaces;
using WorkerService_Executor.Core;
using WorkerService_Executor.Functions;
using WorkerService_Executor.Interfaces;
using Lib.CommonFunctions.Interfaces;

namespace WorkerService_Executor
{
    public class WorkerExecutor : BackgroundService, IWorkerExecutor, IDisposable
    {
        private readonly ILogger<WorkerExecutor> _logger;
        private ParseHelper parseHelper;
        private readonly IAppDbContext _appDbContext;

        private readonly ICommonFunctions _commonFunctions;

        public WorkerExecutor(ILogger<WorkerExecutor> logger, IAppDbContext aAppDbContext, ICommonFunctions aCommonFunctions)
        {
            _logger = logger;
            _appDbContext = aAppDbContext;

            _commonFunctions = aCommonFunctions;
            _commonFunctions.SetLogger(_logger);

            parseHelper = new ParseHelper(_logger, _appDbContext, _commonFunctions);

            // Load settings
            new Settings(_logger, _commonFunctions).ProceedConfigFile();

            // Only once settings has been loaded.
            _appDbContext.SetConnectionString(AppData.ConnectionString);
        }

        public override void Dispose()
        {
            _appDbContext.Dispose();

            base.Dispose();
        }

        public async void StartWithParametersAsync(Message aMessage, CancellationToken cancellationToken)
        {
            //
            _logger.LogInformation("WorkerExecutor started with parameters: FileName {aMessage.FileName}. Id {aMessage.TrackFileId}", aMessage.FileName, aMessage.TrackFileId);

            _logger.LogInformation("Start with file \"{aMessage.FileName}\" at {DateTime.Now}", aMessage.FileName, DateTime.Now);

            // Run
            DataFileResult? dataFileResult = null;

            // Update db -- Update Start Proceed Time
            // After first usage we we get Id
            TrackLog_Files? file;
            file = _appDbContext.TrackLog_Files.Where(x => x.TrackFileId == aMessage.TrackFileId && x.FileStartProceedTime == null).FirstOrDefault();

            if (file == null) 
            {
                _logger.LogError("Entry not found in TrackLog_Files table! File name \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. 1st update", aMessage.FileName, aMessage.TrackFileId);
                return;
            }

            file.FileStartProceedTime = DateTime.Now;

            _appDbContext.SaveChanges();

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
                    file = _appDbContext.TrackLog_Files.Where(x => x.TrackFileId == file.TrackFileId && x.FileStartProceedTime != null && x.FileFinishProceedTime == null).FirstOrDefault();

                    if (file == null)
                    {
                        _logger.LogError("Entry not found in TrackLog_Files table! File name \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. 2nd update", aMessage.FileName, aMessage.TrackFileId);
                    }
                    else
                    {
                        file.ErrorMessage = dataFileResult.ErrorMessaget;
                        file.EntriesInFilesOK = dataFileResult.EntriesInFilesOK;
                        file.EntriesInFilesFailed = dataFileResult.EntriesInFilesFailed;
                        file.OverallSuccessStatus = dataFileResult.Suceeded;

                        file.FileFinishProceedTime = DateTime.Now;
                        _appDbContext.SaveChanges();
                    }
                }
            }

            // End
            _logger.LogInformation(
                "Ended with file \"{aMessage.FileName}\". Id {aMessage.TrackFileId}. Proceeded records: {dataFileResult.EntriesInFilesOK}. Failed records: {dataFileResult.EntriesInFilesFailed}. Overall status: {dataFileResult.Suceeded}",
                aMessage.FileName, aMessage.TrackFileId, dataFileResult.EntriesInFilesOK, dataFileResult.EntriesInFilesFailed, dataFileResult.Suceeded
            );
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
                    _logger.LogError("Exception on Task cancellation. {ex.Message}", ex.Message);
                }
            }
        }
    }
}