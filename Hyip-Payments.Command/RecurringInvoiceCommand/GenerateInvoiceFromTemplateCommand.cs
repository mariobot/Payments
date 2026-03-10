using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Hyip_Payments.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Command.RecurringInvoiceCommand;

/// <summary>
/// Command to generate an invoice from a recurring invoice template
/// </summary>
public class GenerateInvoiceFromTemplateCommand : IRequest<InvoiceWithItemsDto>
{
    public int RecurringInvoiceId { get; set; }
    public DateTime? InvoiceDate { get; set; } // Optional, defaults to today
    public string? UserId { get; set; }
}

public class GenerateInvoiceFromTemplateCommandHandler : IRequestHandler<GenerateInvoiceFromTemplateCommand, InvoiceWithItemsDto>
{
    private readonly PaymentsDbContext _context;
    private readonly IMediator _mediator;
    private readonly InvoiceNumberService _invoiceNumberService;

    public GenerateInvoiceFromTemplateCommandHandler(PaymentsDbContext context, IMediator mediator, InvoiceNumberService invoiceNumberService)
    {
        _context = context;
        _mediator = mediator;
        _invoiceNumberService = invoiceNumberService;
    }

    public async Task<InvoiceWithItemsDto> Handle(GenerateInvoiceFromTemplateCommand request, CancellationToken cancellationToken)
    {
        // Get recurring invoice template
        var recurringInvoice = await _context.RecurringInvoices
            .Include(r => r.TemplateItems)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == request.RecurringInvoiceId, cancellationToken);

        if (recurringInvoice == null)
        {
            throw new InvalidOperationException($"Recurring invoice template with ID {request.RecurringInvoiceId} not found");
        }

        if (!recurringInvoice.IsActive)
        {
            throw new InvalidOperationException($"Recurring invoice template '{recurringInvoice.TemplateName}' is inactive");
        }

        // Check if we've reached end date
        if (recurringInvoice.EndDate.HasValue && DateTime.UtcNow > recurringInvoice.EndDate.Value)
        {
            throw new InvalidOperationException($"Recurring invoice template '{recurringInvoice.TemplateName}' has ended on {recurringInvoice.EndDate.Value:yyyy-MM-dd}");
        }

        var invoiceDate = request.InvoiceDate ?? DateTime.UtcNow;

        // Generate invoice number using the centralized service
        var invoiceNumber = await _invoiceNumberService.GenerateNextInvoiceNumberAsync();

        // Create invoice DTO
        var createInvoiceDto = new InvoiceCommand.InvoiceDto
        {
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate,
            Description = recurringInvoice.InvoiceDescription ?? $"Recurring: {recurringInvoice.TemplateName}",
            CustomerId = recurringInvoice.CustomerId,
            IsActive = true
        };

        // Convert template items to invoice items
        var invoiceItems = recurringInvoice.TemplateItems.Select(item => new InvoiceCommand.InvoiceItemDto
        {
            ProductId = item.ProductId ?? 0,
            ItemName = item.ItemName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList();

        // Create invoice using existing command
        var addInvoiceCommand = new AddInvoiceWithProductsCommand
        {
            Invoice = createInvoiceDto,
            Items = invoiceItems
        };

        var result = await _mediator.Send(addInvoiceCommand, cancellationToken);

        // Update recurring invoice tracking
        recurringInvoice.LastGeneratedDate = invoiceDate;
        recurringInvoice.GeneratedInvoiceCount++;
        recurringInvoice.NextGenerationDate = CalculateNextGenerationDate(invoiceDate, recurringInvoice.Frequency, recurringInvoice.DayOfMonth);
        recurringInvoice.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // TODO: If AutoSendEmail is true, trigger email service here
        if (recurringInvoice.AutoSendEmail)
        {
            Console.WriteLine($"[Recurring Invoice] Auto-send email for invoice {result.InvoiceNumber} to customer {recurringInvoice.Customer.Email}");
            // await _emailService.SendInvoiceEmail(result.InvoiceId);
        }

        return result;
    }

    private DateTime CalculateNextGenerationDate(DateTime currentDate, string frequency, int dayOfMonth)
    {
        DateTime nextDate;

        switch (frequency.ToLower())
        {
            case "monthly":
                nextDate = currentDate.AddMonths(1);
                break;

            case "quarterly":
                nextDate = currentDate.AddMonths(3);
                break;

            case "annually":
                nextDate = currentDate.AddYears(1);
                break;

            default:
                throw new ArgumentException($"Invalid frequency: {frequency}");
        }

        // Adjust to target day of month
        var maxDay = DateTime.DaysInMonth(nextDate.Year, nextDate.Month);
        var targetDay = Math.Min(dayOfMonth, maxDay);

        return new DateTime(nextDate.Year, nextDate.Month, targetDay);
    }
}

/// <summary>
/// Command to manually generate invoice from recurring template (one-time)
/// </summary>
public class ManualGenerateInvoiceCommand : IRequest<InvoiceWithItemsDto>
{
    public int RecurringInvoiceId { get; set; }
    public string? UserId { get; set; }
}

public class ManualGenerateInvoiceCommandHandler : IRequestHandler<ManualGenerateInvoiceCommand, InvoiceWithItemsDto>
{
    private readonly IMediator _mediator;

    public ManualGenerateInvoiceCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<InvoiceWithItemsDto> Handle(ManualGenerateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var generateCommand = new GenerateInvoiceFromTemplateCommand
        {
            RecurringInvoiceId = request.RecurringInvoiceId,
            InvoiceDate = DateTime.UtcNow,
            UserId = request.UserId
        };

        return await _mediator.Send(generateCommand, cancellationToken);
    }
}
