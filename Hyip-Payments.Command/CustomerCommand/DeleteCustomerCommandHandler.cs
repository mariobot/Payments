using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Handler for DeleteCustomerCommand
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public DeleteCustomerCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null)
            return false;

        // Check if customer has any invoices
        var hasInvoices = await _context.Invoices
            .AnyAsync(i => i.CustomerId == request.Id, cancellationToken);

        if (hasInvoices)
        {
            // Don't delete if customer has invoices, just deactivate
            customer.IsActive = false;
            customer.UpdatedAt = DateTime.UtcNow;
            _context.Customers.Update(customer);
        }
        else
        {
            // Safe to delete if no invoices
            _context.Customers.Remove(customer);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
