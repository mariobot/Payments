using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.RecurringInvoiceCommand;

/// <summary>
/// Command to update a recurring invoice template
/// </summary>
public class UpdateRecurringInvoiceCommand : IRequest<RecurringInvoiceModel>
{
    public int Id { get; set; }
    public CreateRecurringInvoiceDto RecurringInvoice { get; set; } = null!;
    public string? UserId { get; set; }
}

public class UpdateRecurringInvoiceCommandHandler : IRequestHandler<UpdateRecurringInvoiceCommand, RecurringInvoiceModel>
{
    private readonly PaymentsDbContext _context;

    public UpdateRecurringInvoiceCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringInvoiceModel> Handle(UpdateRecurringInvoiceCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RecurringInvoice;

        // Get existing recurring invoice
        var recurringInvoice = await _context.RecurringInvoices
            .Include(r => r.TemplateItems)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (recurringInvoice == null)
        {
            throw new InvalidOperationException($"Recurring invoice with ID {request.Id} not found");
        }

        // Validate customer exists
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId && c.IsActive, cancellationToken);

        if (!customerExists)
        {
            throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found or inactive");
        }

        // Update properties
        recurringInvoice.TemplateName = dto.TemplateName;
        recurringInvoice.Description = dto.Description;
        recurringInvoice.CustomerId = dto.CustomerId;
        recurringInvoice.Frequency = dto.Frequency;
        recurringInvoice.DayOfMonth = dto.DayOfMonth;
        recurringInvoice.StartDate = dto.StartDate;
        recurringInvoice.EndDate = dto.EndDate;
        recurringInvoice.InvoiceDescription = dto.InvoiceDescription;
        recurringInvoice.AutoSendEmail = dto.AutoSendEmail;
        recurringInvoice.UpdatedAt = DateTime.UtcNow;
        recurringInvoice.UpdatedByUserId = request.UserId;

        // Remove all existing items
        _context.RecurringInvoiceItems.RemoveRange(recurringInvoice.TemplateItems);

        // Add new items
        foreach (var itemDto in dto.Items)
        {
            var templateItem = new RecurringInvoiceItemModel
            {
                RecurringInvoiceId = recurringInvoice.Id,
                ProductId = itemDto.ProductId,
                ItemName = itemDto.ItemName,
                Description = itemDto.Description,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };

            _context.RecurringInvoiceItems.Add(templateItem);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload navigation properties
        await _context.Entry(recurringInvoice)
            .Collection(r => r.TemplateItems)
            .LoadAsync(cancellationToken);

        await _context.Entry(recurringInvoice)
            .Reference(r => r.Customer)
            .LoadAsync(cancellationToken);

        return recurringInvoice;
    }
}
