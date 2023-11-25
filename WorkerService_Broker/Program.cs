using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using WorkerService_Broker;
using WorkerService_Executor;
using WorkerService_Executor.Interfaces;
using Lib.AppDb.EF;
using Lib.AppDb.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerBroker>();

        // Register IWorkerExecutor and WorkerExecutor.
        services.AddTransient<IWorkerExecutor, WorkerExecutor>();
        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
        services.AddTransient<IAppDbContext, AppDbContext>();
    })
    .Build();

await host.RunAsync();
