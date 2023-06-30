using CartCastle.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartCastle.Services.Core.Common.Queries
{
    public record CustomerOrderDetails(Guid Id, double Total, LineItem[] LineItem,string paymentStatus, Guid transactionId)
    {
        public static CustomerOrderDetails Map(Order order) => new CustomerOrderDetails(order.Id, order.Ordervalue, order.LineItems.ToArray(),order.PaymentStatus, order.TransactionId);
    }
    public record CustomerDetails
    {
        public CustomerDetails() { }
        public CustomerDetails(Guid id, string firstName, string lastName, string email,long mobileNo, CustomerOrderDetails[] customerOrderDetails) 
        { 
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            MobileNo = mobileNo;
            OrderDetails = customerOrderDetails;
        }

        public Guid Id { get; init; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public long MobileNo { get; private set; }
        public CustomerOrderDetails[] OrderDetails { get; init; }
    }
    public record CustomerById(Guid id):IRequest<CustomerDetails>;
    public record AllCustomers : IRequest<IEnumerable<CustomerDetails>>;
}
