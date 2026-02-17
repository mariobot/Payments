using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ProductQuery
{
    /// <summary>
    /// Query to get all active products
    /// By default, only returns products where IsActive = true
    /// </summary>
    public class GetProductListQuery : IRequest<List<ProductModel>>
    {
        public bool IncludeInactive { get; set; } = false; // Optional: Include disabled products
    }

    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, List<ProductModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetProductListQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductModel>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            // Build query with eager loading
            var query = _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .AsQueryable();

            // Filter out inactive products by default
            if (!request.IncludeInactive)
            {
                query = query.Where(p => p.IsActive);
            }

            // Order by name for consistent results
            query = query.OrderBy(p => p.Name);

            return await query.ToListAsync(cancellationToken);
        }
    }
}
