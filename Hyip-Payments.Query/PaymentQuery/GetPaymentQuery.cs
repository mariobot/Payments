using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    // Query to get all payments
    public class GetPaymentQuery : IRequest<List<PaymentModel>>
    {
    }

    // Handler for GetPaymentQuery
    public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, List<PaymentModel>>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetPaymentQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PaymentModel>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
