using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Handler for GetCustomerInvoicesQuery
/// </summary>
public class GetCustomerInvoicesQueryHandler : IRequestHandler<GetCustomerInvoicesQuery, List<InvoiceModel>>
{
    private readonly PaymentsDbContext _context;

    public GetCustomerInvoicesQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<List<InvoiceModel>> Handle(GetCustomerInvoicesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Invoices
            .Include(i => i.Items)
            .Include(i => i.Customer)
            .Where(i => i.CustomerId == request.CustomerId);

        // Filter by status if specified
        if (!string.IsNullOrWhiteSpace(request.StatusFilter))
        {
            query = query.Where(i => i.StatusInvoice == request.StatusFilter);
        }

        // Filter by date range
        if (request.StartDate.HasValue)
        {
            query = query.Where(i => i.InvoiceDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(i => i.InvoiceDate <= request.EndDate.Value);
        }

        // Order by date descending (newest first)
        return await query
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync(cancellationToken);
    }
}
