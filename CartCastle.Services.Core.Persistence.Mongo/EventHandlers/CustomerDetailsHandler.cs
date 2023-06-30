using CartCastle.Common;
using CartCastle.Domain;
using CartCastle.Domain.IntegrationEvents;
using CartCastle.Services.Core.Common.Queries;
using CartCastle.Services.Core.Persistence.Mongo;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using CustomerCreated = CartCastle.Domain.IntegrationEvents.CustomerCreated;

namespace CartCastle.Service.Core.Persistence.Mongo.EventHandlers
{
    public class CustomerDetailsHandler :
        INotificationHandler<CustomerCreated>,
        INotificationHandler<LineItemRemoved>,
        INotificationHandler<LineItemAdded>,
        INotificationHandler<OrderCreated>,
        INotificationHandler<PaymentFailed>,
        INotificationHandler<PaymentSuccessfull>
    {
        private readonly IQueryDbContext _db;
        private readonly IAggregateRepository<Customer, Guid> _customersRepo;
        private readonly IAggregateRepository<Order, Guid> _orderRepo;
        private readonly ILogger<CustomerDetailsHandler> _logger;

        public CustomerDetailsHandler(
            IQueryDbContext db,
            IAggregateRepository<Customer, Guid> customersRepo,
            IAggregateRepository<Order, Guid> ordersRepo,
            ILogger<CustomerDetailsHandler> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _customersRepo = customersRepo ?? throw new ArgumentNullException(nameof(customersRepo));
            _orderRepo = ordersRepo ?? throw new ArgumentNullException(nameof(ordersRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CustomerCreated @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("creating customer details for customer {CustomerId} ...", @event.CustomerId);

            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        public async Task Handle(LineItemRemoved @event, CancellationToken cancellationToken)
        {
            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        public async Task Handle(LineItemAdded @event, CancellationToken cancellationToken)
        {
            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        public async Task Handle(OrderCreated @event, CancellationToken cancellationToken)
        {
            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        public async Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
        {
            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        public async Task Handle(PaymentSuccessfull @event, CancellationToken cancellationToken)
        {
            var customerView = await BuildCustomerViewAsync(@event.CustomerId, cancellationToken);
            await SaveCustomerViewAsync(customerView, cancellationToken);
        }

        private async Task<CustomerDetails> BuildCustomerViewAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var customer = await _customersRepo.RehydrateAsync(customerId, cancellationToken);

            var orders = new CustomerOrderDetails[customer.Orders.Count];
            int index = 0;
            foreach (var id in customer.Orders)
            {
                var order = await _orderRepo.RehydrateAsync(id, cancellationToken);
                orders[index++] = CustomerOrderDetails.Map(order);
            }

            var customerView = new CustomerDetails(customer.Id, customer.FirstName, customer.LastName, customer.Email,customer.MobileNo ,orders);
            return customerView;
        }

        private async Task SaveCustomerViewAsync(CustomerDetails customerView, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDetails>.Filter
                            .Eq(a => a.Id, customerView.Id);

            var update = Builders<CustomerDetails>.Update
                .Set(a => a.Id, customerView.Id)
                .Set(a => a.FirstName, customerView.FirstName)
                .Set(a => a.LastName, customerView.LastName)
                .Set(a => a.Email, customerView.Email)
                .Set(a => a.MobileNo, customerView.MobileNo)
                .Set(a => a.OrderDetails, customerView.OrderDetails);

            await _db.CustomersDetails.UpdateOneAsync(filter,
                cancellationToken: cancellationToken,
                update: update,
                options: new UpdateOptions() { IsUpsert = true });

            _logger.LogInformation($"updated customer details for customer {customerView.Id}");
        }
    }
}