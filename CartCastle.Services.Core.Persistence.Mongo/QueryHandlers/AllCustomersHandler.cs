
using CartCastle.Services.Core.Common.Queries;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartCastle.Services.Core.Persistence.Mongo.QueryHandlers
{
    public class AllCustomersHandler : IRequestHandler<AllCustomers, IEnumerable<CustomerDetails>>
    {
        private readonly IQueryDbContext _queryDbContext;

        public AllCustomersHandler(IQueryDbContext queryDbContext)
        {
            _queryDbContext = queryDbContext;
        }
        public async Task<IEnumerable<CustomerDetails>> Handle(AllCustomers request, CancellationToken cancellationToken)
        {
            var filter = Builders<CustomerDetails>.Filter.Empty;
            var cursor = await _queryDbContext.CustomersDetails.FindAsync(filter, null, cancellationToken);
            return await cursor.ToListAsync(cancellationToken);
        }
    }
}
