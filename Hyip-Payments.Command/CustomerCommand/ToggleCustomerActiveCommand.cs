using MediatR;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Command to toggle customer active status
/// </summary>
public class ToggleCustomerActiveCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string? UpdatedByUserId { get; set; }
}
