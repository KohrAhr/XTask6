using WorkerService_Broker;
using WorkerService_Executor;
using WorkerService_Executor.Interfaces;
using Lib.RabbitMQ;
using Lib.RabbitMQ.Interfaces;
using Lib.AppDb.EF;
using Lib.AppDb.Interfaces;
using Lib.CommonFunctions;
using Lib.CommonFunctions.Interfaces;
using Lib.Parser.Interfaces;
using Lib.Parser;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerBroker>();

        // Register IWorkerExecutor and WorkerExecutor.
        services.AddTransient<IWorkerExecutor, WorkerExecutor>();
        services.AddTransient<IRabbitMQHelper, RabbitMQHelper>();
        services.AddTransient<IAppDbContext, AppDbContext>();
        services.AddTransient<ICommonFunctions, CommonFunctions>();
        services.AddTransient<IParserHelper, ParserHelper>();
    })
    .Build();

await host.RunAsync();
