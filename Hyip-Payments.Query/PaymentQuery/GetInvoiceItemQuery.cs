using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    // Query to get an invoice item by ID
    public class GetInvoiceItemQuery : IRequest<InvoiceItemModel?>
    {
        public int Id { get; }

        public GetInvoiceItemQuery(int id)
        {
            Id = id;
        }
    }

    // Handler for GetInvoiceItemQuery
    public class GetInvoiceItemQueryHandler : IRequestHandler<GetInvoiceItemQuery, InvoiceItemModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetInvoiceItemQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InvoiceItemModel?> Handle(GetInvoiceItemQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.InvoiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken);
        }
    }
}
