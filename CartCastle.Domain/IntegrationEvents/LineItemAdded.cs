using CartCastle.Common.EventBus;
using MediatR;

namespace CartCastle.Domain.IntegrationEvents
{
    public record LineItemAdded : IIntegrationEvent, INotification
    {
        public LineItemAdded(Guid id, LineItem lineItem, Guid orderId, Guid customerId)
        {
            this.Id = id;
            this.LineItem = lineItem;
            this.OrderId = orderId;
            this.CustomerId = customerId;
        }
        public LineItem LineItem { get; init; }
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public Guid Id { get; }
    }
}
