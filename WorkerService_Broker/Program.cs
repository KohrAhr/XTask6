using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using WorkerService_Broker;
using WorkerService_Executor;
using WorkerService_Executor.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerBroker>();

        // Register IWorkerExecutor and WorkerExecutor.
        services.AddTransient<IWorkerExecutor, WorkerExecutor>();
        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
    })
    .Build();

await host.RunAsync();
