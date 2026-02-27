using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.CustomerCommand;

/// <summary>
/// Handler for UpdateCustomerBalanceCommand
/// Automatically calculates customer balance from unpaid invoices
/// </summary>
public class UpdateCustomerBalanceCommandHandler : IRequestHandler<UpdateCustomerBalanceCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public UpdateCustomerBalanceCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCustomerBalanceCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
            return false;

        // Calculate balance from unpaid invoices
        var unpaidInvoices = await _context.Invoices
            .Where(i => i.CustomerId == request.CustomerId)
            .Where(i => i.StatusInvoice != "Paid" && i.StatusInvoice != "Cancelled")
            .ToListAsync(cancellationToken);

        customer.CurrentBalance = unpaidInvoices.Sum(i => i.TotalAmount);
        customer.UpdatedAt = DateTime.UtcNow;

        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
