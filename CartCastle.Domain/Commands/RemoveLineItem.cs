using CartCastle.Common;
using CartCastle.Common.EventBus;
using CartCastle.Domain.IntegrationEvents;
using MediatR;

namespace CartCastle.Domain.Commands
{
    public record RemoveLineItem : IRequest
    {
        public RemoveLineItem(Guid orderId, LineItem lineItem)
        {
            this.OrderId = orderId;
            this.LineItem = lineItem;
        }
        public Guid OrderId { get; internal set; }
        public LineItem LineItem { get; internal set; }
    }

    public class RemoveLineItemHandler : IRequestHandler<RemoveLineItem>
    {
        private readonly IAggregateRepository<Order, Guid> _ordereventsService;
        private readonly IEventProducer _eventProducer;

        public RemoveLineItemHandler(IAggregateRepository<Order, Guid> ordereventsService,
                                  IEventProducer eventProducer)
        {
            _ordereventsService = ordereventsService;
            _eventProducer = eventProducer;
        }
        public async Task Handle(RemoveLineItem request, CancellationToken cancellationToken)
        {
            var order = await _ordereventsService.RehydrateAsync(request.OrderId);
            if (null == order)
                throw new ArgumentOutOfRangeException(nameof(AddLineItem.OrderId), "invalid order id");
            order.RemoveLineItem(request.LineItem);
            await _ordereventsService.PersistAsync(order);

            var @event = new LineItemAdded(Guid.NewGuid(), request.LineItem, order.Id, order.CustomerId);
            await _eventProducer.DispatchAsync(@event);
        }
    }
}
