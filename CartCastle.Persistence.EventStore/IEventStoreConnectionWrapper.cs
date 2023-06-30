using EventStore.ClientAPI;

namespace CartCastle.Persistence.EventStore
{
    public interface IEventStoreConnectionWrapper
    {
        Task<IEventStoreConnection> GetConnectionAsync();
    }
}