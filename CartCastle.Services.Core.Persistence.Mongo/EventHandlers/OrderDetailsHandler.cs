using CartCastle.Common;
using CartCastle.Domain;
using CartCastle.Domain.IntegrationEvents;
using CartCastle.Services.Core.Common.Queries;
using CartCastle.Services.Core.Persistence.Mongo;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CartCastle.Service.Core.Persistence.Mongo.EventHandlers
{
    public class OrderDetailsHandler :
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

        public OrderDetailsHandler(
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

        public async Task Handle(LineItemRemoved @event, CancellationToken cancellationToken)
        {
            var orderView = await BuildOrderViewAsync(@event.OrderId, cancellationToken);
            await SaveOrderViewAsync(orderView, cancellationToken);
        }

        public async Task Handle(LineItemAdded @event, CancellationToken cancellationToken)
        {
            var orderView = await BuildOrderViewAsync(@event.OrderId, cancellationToken);
            await SaveOrderViewAsync(orderView, cancellationToken);
        }

        public async Task Handle(OrderCreated @event, CancellationToken cancellationToken)
        {
            var orderView = await BuildOrderViewAsync(@event.OrderId, cancellationToken);
            await SaveOrderViewAsync(orderView, cancellationToken);
        }

        public async Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
        {
            var orderView = await BuildOrderViewAsync(@event.OrderId, cancellationToken);
            await SaveOrderViewAsync(orderView, cancellationToken);
        }

        public async Task Handle(PaymentSuccessfull @event, CancellationToken cancellationToken)
        {
            var orderView = await BuildOrderViewAsync(@event.OrderId, cancellationToken);
            await SaveOrderViewAsync(orderView, cancellationToken);
        }

        private async Task<OrderDetails> BuildOrderViewAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var order = await _orderRepo.RehydrateAsync(orderId, cancellationToken);


            var orderView = new OrderDetails(order.Id, order.CustomerId, order.Address, order.LineItems.ToArray(), order.Ordervalue, order.TransactionId, order.PaymentStatus);
            return orderView;
        }

        private async Task SaveOrderViewAsync(OrderDetails orderview, CancellationToken cancellationToken)
        {
            var filter = Builders<OrderDetails>.Filter
                            .Eq(a => a.OrderId, orderview.OrderId);

            var update = Builders<OrderDetails>.Update
                .Set(a => a.OrderId, orderview.OrderId)
                .Set(a => a.CustomerId, orderview.CustomerId)
                .Set(a => a.TransactionId, orderview.TransactionId)
                .Set(a => a.Address, orderview.Address)
                .Set(a => a.LineItems, orderview.LineItems)
                .Set(a => a.PaymentStatus, orderview.PaymentStatus)
                .Set(a => a.Ordervalue, orderview.Ordervalue);

            await _db.OrderDetails.UpdateOneAsync(filter,
                cancellationToken: cancellationToken,
                update: update,
                options: new UpdateOptions() { IsUpsert = true });

            _logger.LogInformation($"updated order details for order {orderview.OrderId}");
        }
    }
}