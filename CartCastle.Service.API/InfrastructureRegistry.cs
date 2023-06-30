using CartCastle.Service.Core.Persistence.Mongo.EventHandlers;
using CartCastle.Services.Core.Persistence.Mongo;
using CartCastle.Transport.Kafka;
using MediatR;
using MongoDB.Driver;
using CartCastle.Services.Core.Common;
using CartCastle.Persistence.EventStore;
using System;
using CartCastle.Domain;

namespace CartCastle.Service.API
{
    public static class InfrastructureRegistry
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var eventstoreConnStr = config.GetConnectionString("eventstore");
            var producerConfig = new EventsProducerConfig(config.GetConnectionString("kafka"), config["eventsTopicName"]);

            var mongoConnStr = config.GetConnectionString("mongo");
            var mongoQueryDbName = config["queryDbName"];
            var mongoConfig = new MongoConfig(mongoConnStr, mongoQueryDbName);

            return services.Scan(scan =>
            {
                scan.FromAssembliesOf(typeof(CustomerDetailsHandler))
                    .RegisterHandlers(typeof(IRequestHandler<>))
                    .RegisterHandlers(typeof(IRequestHandler<,>))
                    .RegisterHandlers(typeof(INotificationHandler<>));
            }).AddMongoDb(mongoConfig)
            .AddKafkaEventProducer<Order, Guid>(producerConfig)
            .AddKafkaEventProducer<Customer, Guid>(producerConfig)
            .AddEventStore(eventstoreConnStr);
        }
    }
}