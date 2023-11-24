using Lib.RabbitMQ.Interfaces;
using Lib.RabbitMQ;
using WorkerService_Observer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerObserver>();

        // Register IWorkerExecutor and WorkerExecutor.
        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
    })
    .Build();

await host.RunAsync();
