using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.RecurringInvoiceCommand;

/// <summary>
/// DTO for creating a recurring invoice
/// </summary>
public class CreateRecurringInvoiceDto
{
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CustomerId { get; set; }
    public string Frequency { get; set; } = "Monthly"; // Monthly, Quarterly, Annually
    public int DayOfMonth { get; set; } = 1;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? InvoiceDescription { get; set; }
    public bool AutoSendEmail { get; set; } = false;
    public List<RecurringInvoiceItemDto> Items { get; set; } = new();
}

public class RecurringInvoiceItemDto
{
    public int? ProductId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}

/// <summary>
/// Command to create a recurring invoice template
/// </summary>
public class AddRecurringInvoiceCommand : IRequest<RecurringInvoiceModel>
{
    public CreateRecurringInvoiceDto RecurringInvoice { get; set; } = null!;
    public string? UserId { get; set; }
}

public class AddRecurringInvoiceCommandHandler : IRequestHandler<AddRecurringInvoiceCommand, RecurringInvoiceModel>
{
    private readonly PaymentsDbContext _context;

    public AddRecurringInvoiceCommandHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringInvoiceModel> Handle(AddRecurringInvoiceCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RecurringInvoice;

        // Validate customer exists
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == dto.CustomerId && c.IsActive, cancellationToken);

        if (!customerExists)
        {
            throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found or inactive");
        }

        // Calculate next generation date
        var nextGenerationDate = CalculateNextGenerationDate(dto.StartDate, dto.Frequency, dto.DayOfMonth);

        // Create recurring invoice
        var recurringInvoice = new RecurringInvoiceModel
        {
            TemplateName = dto.TemplateName,
            Description = dto.Description,
            CustomerId = dto.CustomerId,
            Frequency = dto.Frequency,
            DayOfMonth = dto.DayOfMonth,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            InvoiceDescription = dto.InvoiceDescription,
            AutoSendEmail = dto.AutoSendEmail,
            NextGenerationDate = nextGenerationDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = request.UserId,
            GeneratedInvoiceCount = 0
        };

        _context.RecurringInvoices.Add(recurringInvoice);
        await _context.SaveChangesAsync(cancellationToken);

        // Add template items
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

        // Load navigation properties
        await _context.Entry(recurringInvoice)
            .Collection(r => r.TemplateItems)
            .LoadAsync(cancellationToken);

        await _context.Entry(recurringInvoice)
            .Reference(r => r.Customer)
            .LoadAsync(cancellationToken);

        return recurringInvoice;
    }

    private DateTime CalculateNextGenerationDate(DateTime startDate, string frequency, int dayOfMonth)
    {
        var today = DateTime.UtcNow.Date;
        var targetDate = startDate.Date;

        // If start date is in the future, use it
        if (targetDate > today)
            return new DateTime(targetDate.Year, targetDate.Month, Math.Min(dayOfMonth, DateTime.DaysInMonth(targetDate.Year, targetDate.Month)));

        // Calculate next occurrence based on frequency
        switch (frequency.ToLower())
        {
            case "monthly":
                targetDate = new DateTime(today.Year, today.Month, Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year, today.Month)));
                if (targetDate <= today)
                    targetDate = targetDate.AddMonths(1);
                break;

            case "quarterly":
                var currentMonth = today.Month;
                var nextQuarterMonth = ((currentMonth - 1) / 3 + 1) * 3 + 1;
                if (nextQuarterMonth > 12)
                {
                    nextQuarterMonth = 1;
                    targetDate = new DateTime(today.Year + 1, nextQuarterMonth, Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year + 1, nextQuarterMonth)));
                }
                else
                {
                    targetDate = new DateTime(today.Year, nextQuarterMonth, Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year, nextQuarterMonth)));
                }
                if (targetDate <= today)
                    targetDate = targetDate.AddMonths(3);
                break;

            case "annually":
                targetDate = new DateTime(today.Year, startDate.Month, Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year, startDate.Month)));
                if (targetDate <= today)
                    targetDate = targetDate.AddYears(1);
                break;

            default:
                throw new ArgumentException($"Invalid frequency: {frequency}");
        }

        return targetDate;
    }
}
