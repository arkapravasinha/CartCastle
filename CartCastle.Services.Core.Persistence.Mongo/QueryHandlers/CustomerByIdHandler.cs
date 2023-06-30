using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartCastle.Services.Core.Common.Queries;
using MediatR;
using MongoDB.Driver;

namespace CartCastle.Services.Core.Persistence.Mongo.QueryHandlers
{
    public class CustomerByIdHandler : IRequestHandler<CustomerById,CustomerDetails>
    {
        private readonly IQueryDbContext _db;

        public CustomerByIdHandler(IQueryDbContext db)
        {
            _db = db;
        }

        public async Task<CustomerDetails> Handle(CustomerById request, CancellationToken cancellationToken)
        {
            var cursor = await _db.CustomersDetails.FindAsync(c => c.Id == request.id,
                null, cancellationToken);
            return await cursor.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
