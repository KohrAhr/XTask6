using Lib.RabbitMQ.Interfaces;
using Lib.RabbitMQ;
using WorkerService_Observer;
using Lib.AppDb.Interfaces;
using Lib.AppDb.EF;
using Lib.CommonFunctions;
using Lib.CommonFunctions.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerObserver>();

        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
        services.AddTransient<IAppDbContext, AppDbContext>();
        services.AddTransient<ICommonFunctions, CommonFunctions>();
    })
    .Build();

await host.RunAsync();
