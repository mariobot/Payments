using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Query.WalletQuery
{
    // Query
    public class GetWalletQuery : IRequest<WalletModel?>
    {
        public int WalletId { get; set; }

        public GetWalletQuery(int walletId)
        {
            WalletId = walletId;
        }
    }

    // Handler
    public class GetWalletQueryHandler : IRequestHandler<GetWalletQuery, WalletModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetWalletQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<WalletModel?> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            return await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken);
        }
    }
}
