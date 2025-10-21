using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    // Query to get an invoice item by ID
    public class GetPaymentListQuery : IRequest<List<InvoiceItemModel?>>
    {
        public GetPaymentListQuery()
        {
            
        }
    }

    // Handler for GetInvoiceItemQuery
    public class GetInvoiceItemQueryHandler : IRequestHandler<GetPaymentListQuery, List<InvoiceItemModel?>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetInvoiceItemQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InvoiceItemModel?>> Handle(GetPaymentListQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.InvoiceItems.ToListAsync(cancellationToken: cancellationToken);
        }
    }
}
