using CartCastle.Common;
using CartCastle.Common.EventBus;
using CartCastle.Domain.IntegrationEvents;
using MediatR;

namespace CartCastle.Domain.Commands
{
    public record CreateOrder : IRequest
    {
        public CreateOrder(Guid orderId, Guid customerId, List<LineItem> lineItems, string address)
        {
            this.OrderId = orderId;
            this.CustomerId = customerId;
            this.LineItems = lineItems;
            this.Address = address;
        }

        public Guid OrderId { get; }
        public List<LineItem> LineItems { get; }
        public Guid CustomerId { get; }
        public string Address { get; }
    }

    public class CreateOrderHandler : IRequestHandler<CreateOrder>
    {
        private readonly IAggregateRepository<Order, Guid> _ordereventsService;
        private readonly IAggregateRepository<Customer, Guid> _customereventsService;
        private readonly IEventProducer _eventProducer;

        public CreateOrderHandler(IAggregateRepository<Order, Guid> ordereventsService,
                                  IAggregateRepository<Customer, Guid> customereventsService,
                                  IEventProducer eventProducer)
        {
            _ordereventsService = ordereventsService;
            _customereventsService = customereventsService;
            _eventProducer = eventProducer;
        }
        public async Task Handle(CreateOrder request, CancellationToken cancellationToken)
        {
            var customer = await _customereventsService.RehydrateAsync(request.CustomerId);
            if (null == customer)
                throw new ArgumentOutOfRangeException(nameof(CreateOrder.CustomerId), "invalid customer id");
            var order = Order.Create(request.OrderId, customer, request.LineItems, request.Address);
            await _customereventsService.PersistAsync(customer);
            await _ordereventsService.PersistAsync(order);

            var @event = new OrderCreated(Guid.NewGuid(), order.Id,order.CustomerId);
            await _eventProducer.DispatchAsync(@event);
        }
    }
}
