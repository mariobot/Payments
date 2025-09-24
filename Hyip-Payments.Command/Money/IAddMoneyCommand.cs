using MediatR;
using Hyip_Payments.Models;

namespace Hyip_Payments.Command.Money
{
    public interface IAddMoneyCommand : IRequest<MoneyModel>
    {
        MoneyModel Money { get; }
    }
}
