using CartCastle.Common.Models;

namespace CartCastle.Domain.DomainEvents
{
    public static class CustomerEvents
    {
        public record CustomerCreated : BaseDomainEvent<Customer, Guid>
        {
            private CustomerCreated() { }
            public CustomerCreated(Customer customer, string firstName, string lastName, string email, long mobileNo) : base(customer)
            {
                FirstName = firstName; LastName = lastName; Email = email; MobileNo = mobileNo;
            }

            public string FirstName { get; init; }
            public string LastName { get; init; }
            public long MobileNo { get; init; }
            public string Email { get; init; }
        }

        public record OrderAdded : BaseDomainEvent<Customer, Guid>
        {
            private OrderAdded() { }

            public OrderAdded(Customer aggregateRoot, Guid id) : base(aggregateRoot)
            {
                OrderId = id;
            }
            public Guid OrderId { get; init; }
        }
    }
}
