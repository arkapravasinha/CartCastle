using CartCastle.Common.EventBus;
using MediatR;

namespace CartCastle.Domain.IntegrationEvents
{
    public record OrderCreated : IIntegrationEvent, INotification
    {
        public OrderCreated(Guid id, Guid orderId,Guid customerId)
        {
            this.Id = id;
            this.OrderId = orderId;
            this.CustomerId = customerId;
        }
        public Guid Id { get; }
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; private set; }
    }
}
