using WorkerService_Executor;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerExecutor>();
    })
    .Build();

await host.RunAsync();
