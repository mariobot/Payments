using MediatR;
using Hyip_Payments.Models;
using Hyip_Payments.Context;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.InvoiceQuery
{
    // Query to get all invoices
    public class GetAllInvoicesQuery : IRequest<List<InvoiceModel>>
    {
    }

    // Handler for GetAllInvoicesQuery
    public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, List<InvoiceModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetAllInvoicesQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<InvoiceModel>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
