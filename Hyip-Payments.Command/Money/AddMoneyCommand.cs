using MediatR;
using Hyip_Payments.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.Money
{
    // Interface for AddMoneyCommand
    public interface IAddMoneyCommand : IRequest<MoneyModel>
    {
        MoneyModel Money { get; }
    }

    // Implementation of AddMoneyCommand
    public class AddMoneyCommand : IAddMoneyCommand
    {
        public MoneyModel Money { get; }

        public AddMoneyCommand(MoneyModel money)
        {
            Money = money;
        }
    }

    // Handler for AddMoneyCommand
    public class AddMoneyCommandHandler : IRequestHandler<AddMoneyCommand, MoneyModel>
    {
        // Inject your data context or repository here as needed
        // private readonly PaymentsDbContext _context;

        // public AddMoneyCommandHandler(PaymentsDbContext context)
        // {
        //     _context = context;
        // }

        public async Task<MoneyModel> Handle(AddMoneyCommand request, CancellationToken cancellationToken)
        {
            // TODO: Add your logic to persist the money
            // Example:
            // _context.Money.Add(request.Money);
            // await _context.SaveChangesAsync(cancellationToken);
            // return request.Money;

            // For demonstration, just return the money
            return await Task.FromResult(request.Money);
        }
    }
}

