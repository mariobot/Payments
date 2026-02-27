using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Handler for GetCustomerListQuery
/// </summary>
public class GetCustomerListQueryHandler : IRequestHandler<GetCustomerListQuery, List<CustomerModel>>
{
    private readonly PaymentsDbContext _context;

    public GetCustomerListQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerModel>> Handle(GetCustomerListQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Customers
            .Include(c => c.Country)
            .Include(c => c.Invoices)
            .AsQueryable();

        // Filter by active status
        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        // Search filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(c =>
                c.CustomerNumber.ToLower().Contains(searchLower) ||
                c.CompanyName.ToLower().Contains(searchLower) ||
                c.ContactName.ToLower().Contains(searchLower) ||
                c.Email.ToLower().Contains(searchLower));
        }

        // Ordering
        query = request.OrderBy?.ToLower() switch
        {
            "companyname" => request.OrderAscending
                ? query.OrderBy(c => c.CompanyName)
                : query.OrderByDescending(c => c.CompanyName),
            "createdat" => request.OrderAscending
                ? query.OrderBy(c => c.CreatedAt)
                : query.OrderByDescending(c => c.CreatedAt),
            "currentbalance" => request.OrderAscending
                ? query.OrderBy(c => c.CurrentBalance)
                : query.OrderByDescending(c => c.CurrentBalance),
            _ => request.OrderAscending
                ? query.OrderBy(c => c.CustomerNumber)
                : query.OrderByDescending(c => c.CustomerNumber)
        };

        return await query.ToListAsync(cancellationToken);
    }
}
