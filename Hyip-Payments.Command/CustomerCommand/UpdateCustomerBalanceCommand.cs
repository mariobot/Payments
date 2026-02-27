using MediatR;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Command to update customer balance (calculated from invoices)
/// </summary>
public class UpdateCustomerBalanceCommand : IRequest<bool>
{
    public int CustomerId { get; set; }
}
