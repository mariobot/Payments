using MediatR;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Command to delete a customer
/// </summary>
public class DeleteCustomerCommand : IRequest<bool>
{
    public int Id { get; set; }
}
