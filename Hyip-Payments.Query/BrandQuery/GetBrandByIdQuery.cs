using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.BrandQuery
{
    // MediatR query to get a brand by Id
    public class GetBrandByIdQuery : IRequest<BrandModel?>
    {
        public int Id { get; }
        public GetBrandByIdQuery(int id) => Id = id;
    }

    public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetBrandByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<BrandModel?> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        }
    }
}
