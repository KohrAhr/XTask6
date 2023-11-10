using WorkerService_Observer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<WorkerObserver>();
    })
    .Build();

await host.RunAsync();
