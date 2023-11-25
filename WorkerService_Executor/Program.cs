using WorkerService_Executor;
using Lib.AppDb.EF;
using Lib.AppDb.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerExecutor>();

        services.AddTransient<IAppDbContext, AppDbContext>();
    })
    .Build();

await host.RunAsync();
