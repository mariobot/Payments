using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentMethodQuery
{
    /// <summary>
    /// Query to get a payment method by ID
    /// </summary>
    public class GetPaymentMethodByIdQuery : IRequest<PaymentMethodModel?>
    {
        public int Id { get; }

        public GetPaymentMethodByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethodModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentMethodByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodModel?> Handle(GetPaymentMethodByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        }
    }
}
