using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.MoneyCommand
{
    // MediatR command to delete a money record by ID
    public class DeleteMoneyCommand : IRequest<bool>
    {
        public int MoneyId { get; }
        public DeleteMoneyCommand(int moneyId) => MoneyId = moneyId;
    }

    // Handler
    public class DeleteMoneyCommandHandler : IRequestHandler<DeleteMoneyCommand, bool>
    {
        private readonly PaymentsDbContext _context;

        public DeleteMoneyCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMoneyCommand request, CancellationToken cancellationToken)
        {
            var money = await _context.Money
                .FirstOrDefaultAsync(m => m.Id == request.MoneyId, cancellationToken);

            if (money == null)
                return false;

            _context.Money.Remove(money);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
