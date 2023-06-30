using CartCastle.Common;
using CartCastle.Common.Models;
using CartCastle.Common.Serialization;
using CartCastle.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CartCastle.Persistence.EventStore
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services, string connectionString)
        {
            return services.AddSingleton<IEventStoreConnectionWrapper>(ctx =>
                {
                    var logger = ctx.GetRequiredService<ILogger<EventStoreConnectionWrapper>>();
                    return new EventStoreConnectionWrapper(new Uri(connectionString), logger);
                }).AddEventsRepository<Customer, Guid>()
                .AddEventsRepository<Order, Guid>();
        }

        private static IServiceCollection AddEventsRepository<TA, TK>(this IServiceCollection services)
            where TA : class, IAggregateRoot<TK>
        {
            return services.AddSingleton<IAggregateRepository<TA, TK>>(ctx =>
            {
                var connectionWrapper = ctx.GetRequiredService<IEventStoreConnectionWrapper>();
                var eventDeserializer = ctx.GetRequiredService<IEventSerializer>();
                return new AggregateRepository<TA, TK>(connectionWrapper, eventDeserializer);
            });
        }
    }
}