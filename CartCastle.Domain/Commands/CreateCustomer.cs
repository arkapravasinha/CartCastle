using CartCastle.Common;
using CartCastle.Common.EventBus;
using CartCastle.Domain.IntegrationEvents;
using MediatR;

namespace CartCastle.Domain.Commands
{
    public record CreateCustomer : IRequest
    {
        public CreateCustomer(Guid Id, string firstName, string lastName, string email, long mobileNo)
        {
            this.CustomerId = Id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.MobileNo = mobileNo;
        }
        public Guid CustomerId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public long MobileNo { get; }
    }

    public class CreateCustomerHandler : IRequestHandler<CreateCustomer>
    {
        private readonly IAggregateRepository<Customer, Guid> _eventsService;
        private readonly IEventProducer _eventProducer;

        public CreateCustomerHandler(IAggregateRepository<Customer, Guid> eventsService,
                                     IEventProducer eventProducer)
        {
            _eventsService = eventsService;
            _eventProducer = eventProducer;
        }
        public async Task Handle(CreateCustomer request, CancellationToken cancellationToken)
        {
            var customer = Customer.Create(request.CustomerId, request.FirstName, request.LastName, request.Email, request.MobileNo);
            await _eventsService.PersistAsync(customer);
            var @event = new CustomerCreated(Guid.NewGuid(), request.CustomerId);
            await _eventProducer.DispatchAsync(@event);
        }
    }
}
