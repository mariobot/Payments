using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.BrandQuery
{
    // MediatR query to get all brands
    public class GetBrandListQuery : IRequest<List<BrandModel>>
    {
    }

    public class GetBrandListQueryHandler : IRequestHandler<GetBrandListQuery, List<BrandModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetBrandListQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BrandModel>> Handle(GetBrandListQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Brands
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
