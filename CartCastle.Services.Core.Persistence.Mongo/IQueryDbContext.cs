using CartCastle.Services.Core.Common.Queries;
using MongoDB.Driver;

namespace CartCastle.Services.Core.Persistence.Mongo
{
    public interface IQueryDbContext
    {
        IMongoCollection<OrderDetails> OrderDetails { get; }
        IMongoCollection<CustomerDetails> CustomersDetails { get; }
    }
}