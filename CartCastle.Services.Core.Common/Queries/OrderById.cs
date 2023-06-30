using CartCastle.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartCastle.Services.Core.Common.Queries
{
    public record OrderById(Guid id) : IRequest<OrderDetails>;

    public record OrderDetails
    {
        public OrderDetails(Guid id, Guid customerId,string address, LineItem[] lineItems,double orderValue,Guid transactionId, string paymentStatus)
        {
            OrderId = id;
            CustomerId = customerId;
            Address = address;
            LineItems = lineItems;
            Ordervalue = orderValue;
            TransactionId = transactionId;
            PaymentStatus = paymentStatus;
        }
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public string Address { get; init; }
        public LineItem[] LineItems { get; init; }
        public double Ordervalue { get; init; }
        public Guid TransactionId { get; init; }
        public string PaymentStatus { get; init; }
    }
}