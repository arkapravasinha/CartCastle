using CartCastle.Common.Models;
using CartCastle.Domain.DomainEvents;

namespace CartCastle.Domain
{
    public record Customer : BaseAggregateRoot<Customer, Guid>
    {
        private readonly HashSet<Guid> _orders = new();

        private Customer() { }
        public Customer(Guid id, string firstName, string lastName, string email, long mobileNo):base(id)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentNullException(nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentNullException(nameof(lastName));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException(nameof(email));
            if (mobileNo <= 0)
                throw new ArgumentNullException(nameof(mobileNo));
            this.Append(new CustomerEvents.CustomerCreated(this, firstName, lastName, email, mobileNo));

        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public long MobileNo { get; private set; }

        public IReadOnlyCollection<Guid> Orders => _orders;

        public void AddOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(Order));
            if (_orders.Contains(order.Id)) return;
            this.Append(new CustomerEvents.OrderAdded(this, order.Id));
        }
        protected override void When(IDomainEvent<Guid> @event)
        {
            switch (@event)
            {
                case CustomerEvents.CustomerCreated c:
                    this.Id = c.AggregateId;
                    this.FirstName = c.FirstName;
                    this.LastName = c.LastName;
                    this.Email = c.Email;
                    this.MobileNo = c.MobileNo;
                    break;
                case CustomerEvents.OrderAdded a:
                    _orders.Add(a.OrderId);
                    break;
            }
        }

        public static Customer Create(Guid customerId, string firstName, string lastName, string email, long mobileNo)
        {
            return new Customer(customerId, firstName, lastName, email, mobileNo);
        }
    }
}
