namespace CartCastle.Common.Models
{
    public interface IDomainEvent<out TKey>
    {
        long AggregateVersion { get; }
        TKey AggregateId { get; }
        DateTime When { get; }
    }
}
