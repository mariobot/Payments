using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.MoneyCommand
{
    // MediatR command to edit a money record
    public class EditMoneyCommand : IRequest<MoneyModel?>
    {
        public MoneyModel Money { get; }
        public EditMoneyCommand(MoneyModel money) => Money = money;
    }

    // Handler
    public class EditMoneyCommandHandler : IRequestHandler<EditMoneyCommand, MoneyModel?>
    {
        private readonly PaymentsDbContext _context;

        public EditMoneyCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<MoneyModel?> Handle(EditMoneyCommand request, CancellationToken cancellationToken)
        {
            var existing = await _context.Money
                .FirstOrDefaultAsync(m => m.Id == request.Money.Id, cancellationToken);

            if (existing == null)
                return null;

            // Update properties as needed
            existing.Slug = request.Money.Slug;
            existing.Currency = request.Money.Currency;
            existing.Country = request.Money.Country;
            existing.Amount = request.Money.Amount;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }
    }
}
