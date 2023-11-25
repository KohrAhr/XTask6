using Lib.AppDb.EF;
using Lib.AppDb.Interfaces;
using Lib.CommonFunctions.Interfaces;
using Lib.CommonFunctions;
using WorkerService_Executor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerExecutor>();

        services.AddTransient<IAppDbContext, AppDbContext>();
        services.AddTransient<ICommonFunctions, CommonFunctions>();
    })
    .Build();

await host.RunAsync();
