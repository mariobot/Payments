using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.WalletQuery
{
    // MediatR query to get all wallets
    public class GetWalletListQuery : IRequest<List<WalletModel>>
    {
    }

    public class GetWalletListQueryHandler : IRequestHandler<GetWalletListQuery, List<WalletModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetWalletListQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<WalletModel>> Handle(GetWalletListQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Wallets
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
