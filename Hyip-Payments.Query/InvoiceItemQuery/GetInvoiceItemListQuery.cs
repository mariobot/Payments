using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.InvoiceItemQuery
{
    // MediatR query to get all invoice items
    public class GetInvoiceItemListQuery : IRequest<List<InvoiceItemModel>>
    {
    }

    // Handler
    public class GetInvoiceItemListQueryHandler : IRequestHandler<GetInvoiceItemListQuery, List<InvoiceItemModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceItemListQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<InvoiceItemModel>> Handle(GetInvoiceItemListQuery request, CancellationToken cancellationToken)
        {
            return await _context.InvoiceItems
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
