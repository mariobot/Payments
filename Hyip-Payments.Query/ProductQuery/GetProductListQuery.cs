using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ProductQuery
{
    // MediatR query to get all products
    public class GetProductListQuery : IRequest<List<ProductModel>>
    {
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
            return await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync(cancellationToken);
        }
    }
}
