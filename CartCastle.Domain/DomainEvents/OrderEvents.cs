using CartCastle.Common.Models;

namespace CartCastle.Domain.DomainEvents
{
    public static class OrderEvents
    {
        public record OrderCreated : BaseDomainEvent<Order, Guid>
        {
            public Guid CustomerId { get; init; }
            public List<LineItem> LineItems { get; init; }
            public string Address { get; init; }
            public double TotalValue { get; init; }

            private OrderCreated() { }
            public OrderCreated(Order order, Customer customer, List<LineItem> lineItems, string address, double orderValue) : base(order)
            {
                if (customer == null) throw new ArgumentNullException(nameof(customer));
                CustomerId = customer.Id;
                LineItems = lineItems;
                Address = address;
                TotalValue = orderValue;
            }

        }

        public record LineItemAdded : BaseDomainEvent<Order, Guid>
        {
            public LineItem LineItem { get; init; }
            private LineItemAdded() { }
            public LineItemAdded(Order order, LineItem lineItem) : base(order)
            {
                if (lineItem == null) throw new ArgumentNullException(nameof(lineItem));
                LineItem = lineItem;
            }
        }

        public record LineItemRemoved : BaseDomainEvent<Order, Guid>
        {
            public LineItem LineItem { get; init; }
            private LineItemRemoved() { }
            public LineItemRemoved(Order order, LineItem lineItem) : base(order)
            {
                if (lineItem == null) throw new ArgumentNullException(nameof(lineItem));
                LineItem = lineItem;
            }
        }

        public record PaymentSuccessfull : BaseDomainEvent<Order, Guid>
        {
            public string PaymentStatus
            {
                get
                {
                    return "Successfull";
                }
            }

            public Guid TransactionId { get; init; }

            private PaymentSuccessfull() { }
            public PaymentSuccessfull(Order order, Guid TransactionId) : base(order)
            {
                this.TransactionId = TransactionId;
            }
        }

        public record PaymentFailed : BaseDomainEvent<Order, Guid>
        {
            public string PaymentStatus
            {
                get
                {
                    return "PaymentFailed";
                }
            }

            public Guid TransactionId { get; init; }

            private PaymentFailed() { }
            public PaymentFailed(Order order, Guid TransactionId) : base(order)
            {
                this.TransactionId = TransactionId;
            }
        }
    }
}
