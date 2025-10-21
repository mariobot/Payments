using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CoinCommand
{
    // MediatR command to edit a coin
    public class EditCoinCommand : IRequest<CoinModel?>
    {
        public CoinModel Coin { get; }
        public EditCoinCommand(CoinModel coin) => Coin = coin;
    }

    // Handler
    public class EditCoinCommandHandler : IRequestHandler<EditCoinCommand, CoinModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public EditCoinCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CoinModel?> Handle(EditCoinCommand request, CancellationToken cancellationToken)
        {
            var existing = await _dbContext.Coins.FirstOrDefaultAsync(c => c.Id == request.Coin.Id, cancellationToken);
            if (existing == null)
                return null;

            existing.Symbol = request.Coin.Symbol;
            existing.Name = request.Coin.Name;
            existing.Description = request.Coin.Description;
            existing.CurrentPrice = request.Coin.CurrentPrice;
            existing.Network = request.Coin.Network;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
