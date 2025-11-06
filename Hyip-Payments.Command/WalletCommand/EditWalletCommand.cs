using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.WalletCommand
{
    // Command
    public class EditWalletCommand : IRequest<WalletModel?>
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string? UserId { get; set; }
        public bool IsActive { get; set; }

        public EditWalletCommand() { }

        public EditWalletCommand(WalletModel wallet)
        {
            Id = wallet.Id;
            Address = wallet.Address;
            Currency = wallet.Currency;
            Balance = wallet.Balance;
            UserId = wallet.UserId;
            IsActive = wallet.IsActive;
        }
    }

    // Handler
    public class EditWalletCommandHandler : IRequestHandler<EditWalletCommand, WalletModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditWalletCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<WalletModel?> Handle(EditWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == request.Id, cancellationToken);
            if (wallet == null)
                return null;

            wallet.Address = request.Address;
            wallet.Currency = request.Currency;
            wallet.Balance = request.Balance;
            wallet.UserId = request.UserId;
            wallet.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return wallet;
        }
    }
}
