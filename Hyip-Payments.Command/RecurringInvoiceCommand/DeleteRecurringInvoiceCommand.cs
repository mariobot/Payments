using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.RecurringInvoiceCommand;

/// <summary>
/// Command to delete a recurring invoice template
/// </summary>
public class DeleteRecurringInvoiceCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteRecurringInvoiceCommandHandler : IRequestHandler<DeleteRecurringInvoiceCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public DeleteRecurringInvoiceCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteRecurringInvoiceCommand request, CancellationToken cancellationToken)
    {
        var recurringInvoice = await _context.RecurringInvoices
            .Include(r => r.TemplateItems)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (recurringInvoice == null)
        {
            return false;
        }

        // Remove items first
        _context.RecurringInvoiceItems.RemoveRange(recurringInvoice.TemplateItems);

        // Remove recurring invoice
        _context.RecurringInvoices.Remove(recurringInvoice);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

/// <summary>
/// Command to toggle active status of recurring invoice
/// </summary>
public class ToggleRecurringInvoiceActiveCommand : IRequest<bool>
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
}

public class ToggleRecurringInvoiceActiveCommandHandler : IRequestHandler<ToggleRecurringInvoiceActiveCommand, bool>
{
    private readonly PaymentsDbContext _context;

    public ToggleRecurringInvoiceActiveCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ToggleRecurringInvoiceActiveCommand request, CancellationToken cancellationToken)
    {
        var recurringInvoice = await _context.RecurringInvoices
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (recurringInvoice == null)
        {
            return false;
        }

        recurringInvoice.IsActive = request.IsActive;
        recurringInvoice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
