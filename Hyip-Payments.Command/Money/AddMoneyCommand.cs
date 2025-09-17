using MediatR;

using Hyip_Payments.Models;

namespace Hyip_Payments.Command.Money;

public class AddMoneyCommand : IRequest<MoneyModel>
{
    public MoneyModel Money { get; }

    public AddMoneyCommand(MoneyModel money)
    {
        Money = money;
    }
}
