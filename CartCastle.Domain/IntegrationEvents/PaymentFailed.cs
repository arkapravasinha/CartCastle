using CartCastle.Common.EventBus;
using MediatR;

namespace CartCastle.Domain.IntegrationEvents
{
    public record PaymentFailed : IIntegrationEvent, INotification
    {
        public PaymentFailed(Guid id, Guid transactionId, Guid orderId, Guid customerId)
        {
            this.Id = id;
            this.TransactionId = transactionId;
            this.OrderId = orderId;
            this.CustomerId = customerId;
        }
        public Guid Id { get; }
        public Guid TransactionId { get; init; }
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
    }
}
