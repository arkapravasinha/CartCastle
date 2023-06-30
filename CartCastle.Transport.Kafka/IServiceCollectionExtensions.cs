using CartCastle.Common.EventBus;
using CartCastle.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CartCastle.Transport.Kafka
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaEventProducer<TA, TK>(this IServiceCollection services, EventsProducerConfig configuration)
            where TA : class, IAggregateRoot<TK>
        {
            return services.AddSingleton<IEventProducer>(ctx =>
            {
                var logger = ctx.GetRequiredService<ILogger<EventProducer>>();
                return new EventProducer(configuration.TopicName, configuration.KafkaConnectionString, logger);
            });
        }
    }
}