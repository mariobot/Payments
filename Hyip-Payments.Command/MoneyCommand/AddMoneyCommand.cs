using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;

namespace Hyip_Payments.Command.MoneyCommand
{
    // MediatR command to add money
    public class AddMoneyCommand : IRequest<MoneyModel>
    {
        public MoneyModel Money { get; }
        public AddMoneyCommand(MoneyModel money) => Money = money;
    }

    // Optional interface for abstraction/testing
    public interface IAddMoneyCommandHandler
    {
        Task<MoneyModel> Handle(AddMoneyCommand request, CancellationToken cancellationToken);
    }

    // Handler
    public class AddMoneyCommandHandler : IRequestHandler<AddMoneyCommand, MoneyModel>, IAddMoneyCommandHandler
    {
        private readonly PaymentsDbContext _context;

        public AddMoneyCommandHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<MoneyModel> Handle(AddMoneyCommand request, CancellationToken cancellationToken)
        {
            _context.Money.Add(request.Money);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Money;
        }
    }
}
