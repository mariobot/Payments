using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.CoinCommand
{
    // MediatR command to add a coin
    public class AddCoinCommand : IRequest<CoinModel>
    {
        public CoinModel Coin { get; }
        public AddCoinCommand(CoinModel coin) => Coin = coin;
    }

    public class AddCoinCommandHandler : IRequestHandler<AddCoinCommand, CoinModel>
    {
        private readonly PaymentsDbContext _dbContext;

        public AddCoinCommandHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CoinModel> Handle(AddCoinCommand request, CancellationToken cancellationToken)
        {
            _dbContext.Coins.Add(request.Coin);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return request.Coin;
        }
    }
}
