using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentMethodQuery
{
    /// <summary>
    /// Query to get all payment methods
    /// </summary>
    public class GetPaymentMethodListQuery : IRequest<List<PaymentMethodModel>>
    {
    }

    public class GetPaymentMethodListQueryHandler : IRequestHandler<GetPaymentMethodListQuery, List<PaymentMethodModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentMethodListQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentMethodModel>> Handle(GetPaymentMethodListQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
