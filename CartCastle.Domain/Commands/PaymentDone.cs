using CartCastle.Common;
using CartCastle.Common.EventBus;
using CartCastle.Domain.IntegrationEvents;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartCastle.Domain.Commands
{
    public record PaymentDone : IRequest
    {
        public PaymentDone(Guid orderId, Guid transactionId, bool isSuccessfull)
        {
            OrderId = orderId;
            TransactionId = transactionId;
            IsSuccess = isSuccessfull;
        }

        public Guid OrderId { get; internal set; }
        public Guid TransactionId { get; internal set; }
        public bool IsSuccess { get; internal set; }
    }

    public class PaymentDoneHandler: IRequestHandler<PaymentDone>
    {
        private readonly IAggregateRepository<Order, Guid> _ordereventsService;
        private readonly IEventProducer _eventProducer;

        public PaymentDoneHandler(IAggregateRepository<Order,Guid> ordereventsService,
                                  IEventProducer eventProducer) {
            _ordereventsService = ordereventsService;
            _eventProducer = eventProducer;
        }

        public async Task Handle(PaymentDone request, CancellationToken cancellationToken)
        {
            var order = await _ordereventsService.RehydrateAsync(request.OrderId);
            if (null == order)
                throw new ArgumentOutOfRangeException(nameof(PaymentDone.OrderId), "invalid order id");
            IIntegrationEvent @event = null;
            if (request.IsSuccess)
            {
                order.PaymentSuccessfull(request.TransactionId);
                @event=new PaymentSuccessfull(Guid.NewGuid(),request.TransactionId,order.Id, order.CustomerId);
            }
            else
            {
                order.PaymentFailed(request.TransactionId);
                @event = new PaymentFailed(Guid.NewGuid(), request.TransactionId, order.Id, order.CustomerId);
            }

            await _ordereventsService.PersistAsync(order);
            await _eventProducer.DispatchAsync(@event);
        }
    }
}
