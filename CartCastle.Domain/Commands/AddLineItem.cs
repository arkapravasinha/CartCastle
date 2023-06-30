using CartCastle.Common;
using CartCastle.Common.EventBus;
using CartCastle.Domain.IntegrationEvents;
using MediatR;

namespace CartCastle.Domain.Commands
{
    public record AddLineItem : IRequest
    {
        public AddLineItem(Guid orderId, LineItem lineItem)
        {
            this.OrderId = orderId;
            this.LineItem = lineItem;
        }
        public Guid OrderId { get; }
        public LineItem LineItem { get; }
    }

    public class AddLineItemHandler : IRequestHandler<AddLineItem>
    {
        private readonly IAggregateRepository<Order, Guid> _ordereventsService;
        private readonly IEventProducer _eventProducer;

        public AddLineItemHandler(IAggregateRepository<Order, Guid> ordereventsService,
                                  IEventProducer eventProducer)
        {
            _ordereventsService = ordereventsService;
            _eventProducer = eventProducer;
        }
        public async Task Handle(AddLineItem request, CancellationToken cancellationToken)
        {
            var order = await _ordereventsService.RehydrateAsync(request.OrderId);
            if (null == order)
                throw new ArgumentOutOfRangeException(nameof(AddLineItem.OrderId), "invalid order id");
            order.AddLineItem(request.LineItem);
            await _ordereventsService.PersistAsync(order);

            var @event = new LineItemAdded(Guid.NewGuid(), request.LineItem, order.Id,order.CustomerId);
            await _eventProducer.DispatchAsync(@event);
        }
    }
}
