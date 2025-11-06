using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.WalletCommand
{
    // Command
    public class DisableWalletCommand : IRequest<bool>
    {
        public int WalletId { get; set; }

        public DisableWalletCommand(int walletId)
        {
            WalletId = walletId;
        }
    }

    // Handler
    public class DisableWalletCommandHandler : IRequestHandler<DisableWalletCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DisableWalletCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DisableWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == request.WalletId, cancellationToken);
            if (wallet == null)
                return false;

            wallet.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
