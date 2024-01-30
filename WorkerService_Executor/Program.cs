using Lib.AppDb.EF;
using Lib.AppDb.Interfaces;
using Lib.CommonFunctions.Interfaces;
using Lib.CommonFunctions;
using WorkerService_Executor;
using Lib.Parser;
using Lib.Parser.Interfaces;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerExecutor>();

        services.AddTransient<IAppDbContext, AppDbContext>();
        services.AddTransient<ICommonFunctions, CommonFunctions>();
        services.AddTransient<IParserHelper, ParserHelper>();
    })
    .Build();

await host.RunAsync();
