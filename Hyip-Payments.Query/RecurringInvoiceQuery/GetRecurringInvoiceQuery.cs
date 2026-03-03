using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.RecurringInvoiceQuery;

/// <summary>
/// Query to get list of recurring invoices
/// </summary>
public class GetRecurringInvoiceListQuery : IRequest<List<RecurringInvoiceModel>>
{
    public int? CustomerId { get; set; }
    public bool? IsActive { get; set; }
    public string? Frequency { get; set; }
}

public class GetRecurringInvoiceListQueryHandler : IRequestHandler<GetRecurringInvoiceListQuery, List<RecurringInvoiceModel>>
{
    private readonly PaymentsDbContext _context;

    public GetRecurringInvoiceListQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecurringInvoiceModel>> Handle(GetRecurringInvoiceListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.RecurringInvoices
            .Include(r => r.Customer)
            .Include(r => r.TemplateItems)
            .AsQueryable();

        if (request.CustomerId.HasValue)
        {
            query = query.Where(r => r.CustomerId == request.CustomerId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(r => r.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(request.Frequency))
        {
            query = query.Where(r => r.Frequency == request.Frequency);
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);
    }
}

/// <summary>
/// Query to get recurring invoice by ID
/// </summary>
public class GetRecurringInvoiceByIdQuery : IRequest<RecurringInvoiceModel?>
{
    public int Id { get; set; }
}

public class GetRecurringInvoiceByIdQueryHandler : IRequestHandler<GetRecurringInvoiceByIdQuery, RecurringInvoiceModel?>
{
    private readonly PaymentsDbContext _context;

    public GetRecurringInvoiceByIdQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<RecurringInvoiceModel?> Handle(GetRecurringInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        return await _context.RecurringInvoices
            .Include(r => r.Customer)
            .Include(r => r.TemplateItems)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
    }
}

/// <summary>
/// Query to get recurring invoices due for generation
/// </summary>
public class GetRecurringInvoicesDueQuery : IRequest<List<RecurringInvoiceModel>>
{
    public DateTime? AsOfDate { get; set; } // Check as of this date
}

public class GetRecurringInvoicesDueQueryHandler : IRequestHandler<GetRecurringInvoicesDueQuery, List<RecurringInvoiceModel>>
{
    private readonly PaymentsDbContext _context;

    public GetRecurringInvoicesDueQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<List<RecurringInvoiceModel>> Handle(GetRecurringInvoicesDueQuery request, CancellationToken cancellationToken)
    {
        var checkDate = request.AsOfDate ?? DateTime.UtcNow.Date;

        var dueInvoices = await _context.RecurringInvoices
            .Include(r => r.Customer)
            .Include(r => r.TemplateItems)
            .Where(r => r.IsActive)
            .Where(r => r.NextGenerationDate <= checkDate)
            .Where(r => !r.EndDate.HasValue || r.EndDate.Value >= checkDate)
            .ToListAsync(cancellationToken);

        return dueInvoices;
    }
}
