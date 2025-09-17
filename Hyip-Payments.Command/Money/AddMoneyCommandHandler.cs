using MediatR;
using Hyip_Payments.Models;
//using Hyip_Payments. Server.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Hyip_Payments.Command.Money;

public class AddMoneyCommandHandler : IRequestHandler<AddMoneyCommand, MoneyModel>
{
    //private readonly ApplicationDbContext _context;

    public AddMoneyCommandHandler(/*ApplicationDbContext context*/)
    {
        //_context = context;
    }

    public async Task<MoneyModel> Handle(AddMoneyCommand request, CancellationToken cancellationToken)
    {
        //_context.Money.Add(request.Money);
        //await _context.SaveChangesAsync(cancellationToken);
        await request.Money.ProcessAsync(); // Simulate some async processing
        return request.Money;
    }
}
