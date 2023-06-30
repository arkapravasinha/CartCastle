using CartCastle.Worker.Notification;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<EventsConsumerWorker>();
    })
    .Build();

host.Run();
