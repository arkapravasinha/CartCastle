using CartCastle.Common.EventBus;
using MediatR;

namespace CartCastle.Domain.IntegrationEvents
{
    public record LineItemRemoved : IIntegrationEvent, INotification
    {
        public LineItemRemoved(Guid id, LineItem lineItem, Guid orderId, Guid customerId)
        {
            this.LineItem = lineItem;
            this.Id = id;
            this.OrderId = orderId;
            this.CustomerId = customerId;
        }

        public LineItem LineItem { get; init; }
        public Guid Id { get; }
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
    }
}
