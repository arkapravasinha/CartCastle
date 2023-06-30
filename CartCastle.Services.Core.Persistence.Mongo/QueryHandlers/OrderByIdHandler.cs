using CartCastle.Services.Core.Common.Queries;
using MediatR;
using MongoDB.Driver;

namespace CartCastle.Services.Core.Persistence.Mongo.QueryHandlers
{
    public class OrderByIdHandler: IRequestHandler<OrderById, OrderDetails>
    {
        private readonly IQueryDbContext _db;
        public OrderByIdHandler(IQueryDbContext db)
        {
            _db = db;
        }

        public async Task<OrderDetails> Handle(OrderById request, CancellationToken cancellationToken)
        {
            var cursor = await _db.OrderDetails.FindAsync(c => c.OrderId == request.id,
                null, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
