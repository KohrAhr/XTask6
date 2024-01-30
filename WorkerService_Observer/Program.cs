using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using WorkerService_Observer;
using WorkerService_Observer.EF;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<AppDbContext>();
        services.AddTransient<RabbitMQHelper>();
        services.AddHostedService<WorkerObserver>();
    })
    .Build();

await host.RunAsync();
