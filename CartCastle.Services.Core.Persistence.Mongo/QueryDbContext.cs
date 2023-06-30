using CartCastle.Services.Core.Common.Queries;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CartCastle.Services.Core.Persistence.Mongo
{
    public class QueryDbContext : IQueryDbContext
    {
        private readonly IMongoDatabase _db;

        private static readonly IBsonSerializer guidSerializer = new GuidSerializer(GuidRepresentation.Standard);
        private static readonly IBsonSerializer<decimal> decimalSerializer = new DecimalSerializer(BsonType.Decimal128);
        private static readonly IBsonSerializer<double> doubleSerializer=new DoubleSerializer(BsonType.Double);
        private static readonly IBsonSerializer<int> intSerializer = new Int32Serializer(BsonType.Int32);



        static QueryDbContext()
        {
            RegisterMappings();
        }

        public QueryDbContext(IMongoDatabase db)
        {
            _db = db ?? throw new System.ArgumentNullException(nameof(db));            

            OrderDetails = _db.GetCollection<OrderDetails>("orderdetails");
            CustomersDetails = _db.GetCollection<CustomerDetails>("customerdetails");
        }

        private static void RegisterMappings()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Domain.LineItem)))
                BsonClassMap.RegisterClassMap<Domain.LineItem>(mapper =>
                {
                    mapper.MapProperty(c => c.ProductId).SetSerializer(guidSerializer);
                    mapper.MapProperty(c => c.PricePerQuantity).SetSerializer(doubleSerializer);
                    mapper.MapProperty(c => c.Qty).SetSerializer(intSerializer);
                    mapper.MapCreator(c => new Domain.LineItem(c.ProductId, c.Qty,c.PricePerQuantity));
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(OrderDetails)))
                BsonClassMap.RegisterClassMap<OrderDetails>(mapper =>
                {
                    mapper.MapIdProperty(c => c.OrderId).SetSerializer(guidSerializer);
                    mapper.MapProperty(c=>c.CustomerId).SetSerializer(guidSerializer);
                    mapper.MapProperty(c => c.Address);
                    mapper.MapProperty(c => c.LineItems);
                    mapper.MapProperty(c => c.Ordervalue).SetSerializer(doubleSerializer);
                    mapper.MapProperty(c => c.PaymentStatus);
                    mapper.MapProperty(c => c.TransactionId).SetSerializer(guidSerializer);
                    mapper.MapCreator(c => new OrderDetails(c.OrderId, c.CustomerId, c.Address, c.LineItems, c.Ordervalue, c.TransactionId,c.PaymentStatus));
                });

            if (!BsonClassMap.IsClassMapRegistered(typeof(CustomerDetails)))
                BsonClassMap.RegisterClassMap<CustomerDetails>(mapper =>
                {
                    mapper.MapIdProperty(c => c.Id).SetSerializer(guidSerializer);
                    mapper.MapProperty(c => c.FirstName);
                    mapper.MapProperty(c => c.LastName);
                    mapper.MapProperty(c => c.Email);
                    mapper.MapProperty(c => c.MobileNo);
                    mapper.MapProperty(c => c.OrderDetails);
                    mapper.MapCreator(c => new CustomerDetails(c.Id, c.FirstName, c.LastName, c.Email, c.MobileNo, c.OrderDetails));
                });
        }

        public IMongoCollection<CustomerDetails> CustomersDetails { get; }

        public IMongoCollection<OrderDetails> OrderDetails { get; }
    }
}