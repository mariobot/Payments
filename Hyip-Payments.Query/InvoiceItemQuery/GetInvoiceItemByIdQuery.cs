using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.InvoiceItemQuery
{
    // MediatR query to get an invoice item by ID
    public class GetInvoiceItemByIdQuery : IRequest<InvoiceItemModel?>
    {
        public int InvoiceItemId { get; }
        public GetInvoiceItemByIdQuery(int invoiceItemId) => InvoiceItemId = invoiceItemId;
    }

    // Handler
    public class GetInvoiceItemByIdQueryHandler : IRequestHandler<GetInvoiceItemByIdQuery, InvoiceItemModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceItemByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemModel?> Handle(GetInvoiceItemByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.InvoiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceItemId, cancellationToken);
        }
    }
}
