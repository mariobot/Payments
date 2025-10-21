using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.InvoiceQuery
{
    // Query to get invoice by ID
    public class GetInvoiceByIdQuery : IRequest<InvoiceModel?>
    {
        public int Id { get; }

        public GetInvoiceByIdQuery(int id)
        {
            Id = id;
        }
    }

    // Handler for GetInvoiceByIdQuery
    public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetInvoiceByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<InvoiceModel?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        }
    }
}
