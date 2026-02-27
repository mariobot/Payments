using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Handler for ToggleCustomerActiveCommand
/// </summary>
public class ToggleCustomerActiveCommandHandler : IRequestHandler<ToggleCustomerActiveCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public ToggleCustomerActiveCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleCustomerActiveCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
            return false;

        customer.IsActive = !customer.IsActive;
        customer.UpdatedAt = DateTime.UtcNow;
        customer.UpdatedByUserId = request.UpdatedByUserId;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
