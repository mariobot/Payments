using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.CustomerQuery;

/// <summary>
/// Handler for GetCustomerByIdQuery
/// </summary>
public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerModel?>
{
    private readonly PaymentsDbContext _context;

    public GetCustomerByIdQueryHandler(PaymentsDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerModel?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsQueryable();

        // Include country if requested
        if (request.IncludeCountry)
        {
            query = query.Include(c => c.Country);
        }

        // Include invoices if requested
        if (request.IncludeInvoices)
        {
            query = query.Include(c => c.Invoices);
        }

        return await query.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    }
}
