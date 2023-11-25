using Lib.RabbitMQ.Interfaces;
using Lib.RabbitMQ;
using WorkerService_Observer;
using Lib.AppDb.Interfaces;
using Lib.AppDb.EF;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerObserver>();

        // Register IWorkerExecutor and WorkerExecutor.
        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
        services.AddTransient<IAppDbContext, AppDbContext>();
    })
    .Build();

await host.RunAsync();
