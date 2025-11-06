using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.WalletCommand
{
    // Command
    public class AddWalletCommand : IRequest<WalletModel>
    {
        public string Address { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string? UserId { get; set; }
        public bool IsActive { get; set; } = true;

        public AddWalletCommand() { }

        public AddWalletCommand(WalletModel wallet)
        {
            Address = wallet.Address;
            Currency = wallet.Currency;
            Balance = wallet.Balance;
            UserId = wallet.UserId;
            IsActive = wallet.IsActive;
        }
    }

    // Handler
    public class AddWalletCommandHandler : IRequestHandler<AddWalletCommand, WalletModel>
    {
        private readonly PaymentsDbContext _context;

        public AddWalletCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<WalletModel> Handle(AddWalletCommand request, CancellationToken cancellationToken)
        {
            var wallet = new WalletModel
            {
                Address = request.Address,
                Currency = request.Currency,
                Balance = request.Balance,
                UserId = request.UserId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync(cancellationToken);

            return wallet;
        }
    }
}
