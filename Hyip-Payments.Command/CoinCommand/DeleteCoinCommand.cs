using Hyip_Payments.Context;
using MediatR;

namespace Hyip_Payments.Command.CoinCommand
{
    // Command
    public class DeleteCoinCommand : IRequest<bool>
    {
        public int CoinId { get; }
        public DeleteCoinCommand(int coinId) => CoinId = coinId;
    }

    // Handler
    public class DeleteCoinCommandHandler : IRequestHandler<DeleteCoinCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteCoinCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCoinCommand request, CancellationToken cancellationToken)
        {
            var coin = await _context.Coins.FindAsync(new object[] { request.CoinId }, cancellationToken);
            if (coin == null)
                return false;

            _context.Coins.Remove(coin);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
