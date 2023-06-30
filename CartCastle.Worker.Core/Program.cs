using CartCastle.Services.Core.Persistence.Mongo;
using CartCastle.Service.Core.Persistence.Mongo.EventHandlers;
using CartCastle.Transport.Kafka;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using CartCastle.Domain.DomainEvents;
using CartCastle.Common.EventBus;
using MediatR;
using CartCastle.Services.Core.Common;
using CartCastle.Services.Core.Common.EventHandlers;
using CartCastle.Common.Serialization;
using CartCastle.Persistence.EventStore;
using Microsoft.AspNetCore.Hosting;
using CartCastle.Worker.Core;

await Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configurationBuilder =>
    {
        configurationBuilder.AddCommandLine(args);
    })
    .ConfigureAppConfiguration((ctx, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .UseSerilog((ctx, cfg) =>
    {
        cfg.Enrich.FromLogContext()
            .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
            .WriteTo.Console();

        var connStr = ctx.Configuration.GetConnectionString("loki");
        cfg.WriteTo.GrafanaLoki(connStr);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var kafkaConnStr = hostContext.Configuration.GetConnectionString("kafka");
        var eventsTopicName = hostContext.Configuration["eventsTopicName"];
        var groupName = hostContext.Configuration["eventsTopicGroupName"];
        var consumerConfig = new EventsConsumerConfig(kafkaConnStr, eventsTopicName, groupName);

        var mongoConnStr = hostContext.Configuration.GetConnectionString("mongo");
        var mongoQueryDbName = hostContext.Configuration["queryDbName"];
        var mongoConfig = new MongoConfig(mongoConnStr, mongoQueryDbName);

        var eventstoreConnStr = hostContext.Configuration.GetConnectionString("eventstore");

        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(OrderDetailsHandler))
                .RegisterHandlers(typeof(INotificationHandler<>));
        }).Decorate(typeof(INotificationHandler<>), typeof(RetryDecorator<>))
            .AddScoped<IMediator, Mediator>()
            .AddSingleton<IEventSerializer>(new JsonEventSerializer(new[]
            {
                typeof(CustomerEvents.CustomerCreated).Assembly
            }))
            .AddSingleton(consumerConfig)
            .AddSingleton(typeof(IEventConsumer), typeof(EventConsumer))
            .AddMongoDb(mongoConfig)
            .AddEventStore(eventstoreConnStr)
            .AddHostedService<EventsConsumerWorker>(); ;
    })
    .Build()
    .RunAsync();