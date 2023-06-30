using CartCastle.Common.EventBus;
using MediatR;

namespace CartCastle.Domain.IntegrationEvents
{
    public record CustomerCreated : IIntegrationEvent, INotification
    {
        public CustomerCreated(Guid id, Guid customerId)
        {
            this.Id = id;
            this.CustomerId = customerId;
        }
        public Guid Id { get; }
        public Guid CustomerId { get; }
    }
}
