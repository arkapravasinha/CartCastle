using CartCastle.Common.Models;
using CartCastle.Domain.DomainEvents;

namespace CartCastle.Domain
{
    public record Order : BaseAggregateRoot<Order, Guid>
    {
        private readonly List<LineItem> _lineItems = new();
        private Order() { }
        public Order(Guid id, Customer customer, List<LineItem> lineItems, string address) : base(id)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));
            if (lineItems == null)
                throw new ArgumentNullException(nameof(lineItems));
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            this.Append(new OrderEvents.OrderCreated(this, customer, lineItems, address, lineItems.Sum(x => (x.Qty * x.PricePerQuantity))));
        }
        public Guid CustomerId { get; private set; }
        public string Address { get; private set; }
        public IReadOnlyCollection<LineItem> LineItems => _lineItems;
        public double Ordervalue => _lineItems.Sum(x => (x.Qty * x.PricePerQuantity));

        public Guid TransactionId { get; private set; }
        public string PaymentStatus { get; private set; }
        public void AddLineItem(LineItem lineItem)
        {
            if (lineItem == null) throw new ArgumentNullException(nameof(lineItem));
            this.Append(new OrderEvents.LineItemAdded(this, lineItem));
        }

        public void RemoveLineItem(LineItem lineItem)
        {
            if (lineItem == null) throw new ArgumentNullException(nameof(lineItem));
            this.Append(new OrderEvents.LineItemRemoved(this, lineItem));
        }

        public void PaymentSuccessfull(Guid transactionId)
        {
            if (transactionId == null || transactionId == Guid.Empty) throw new ArgumentNullException(nameof(transactionId));
            this.Append(new OrderEvents.PaymentSuccessfull(this, transactionId));
        }

        public void PaymentFailed(Guid transactionId)
        {
            if (transactionId == null || transactionId == Guid.Empty) throw new ArgumentNullException(nameof(transactionId));
            this.Append(new OrderEvents.PaymentFailed(this, transactionId));
        }
        protected override void When(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case OrderEvents.OrderCreated c:
                    this.CustomerId = c.CustomerId;
                    this.Address = c.Address;
                    this.Id = c.AggregateId;
                    this._lineItems.AddRange(c.LineItems);
                    break;
                case OrderEvents.LineItemAdded lia:
                    this._lineItems.Add(lia.LineItem);
                    break;
                case OrderEvents.LineItemRemoved lia:
                    this._lineItems.Remove(lia.LineItem);
                    break;
                case OrderEvents.PaymentSuccessfull pay:
                    this.TransactionId = pay.TransactionId;
                    this.PaymentStatus = pay.PaymentStatus;
                    break;
                case OrderEvents.PaymentFailed payf:
                    this.TransactionId = payf.TransactionId;
                    this.PaymentStatus = payf.PaymentStatus;
                    break;


            }
        }

        public static Order Create(Guid orderId, Customer customer, List<LineItem> lineItems, string address)
        {
            var order = new Order(orderId, customer, lineItems, address);
            customer.AddOrder(order);
            return order;
        }
    }
}