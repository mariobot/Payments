using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CoinQuery
{
    // Query
    public class GetCoinsQuery : IRequest<List<CoinModel>>
    {
    }

    // Handler
    public class GetCoinsQueryHandler : IRequestHandler<GetCoinsQuery, List<CoinModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetCoinsQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CoinModel>> Handle(GetCoinsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Coins
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
