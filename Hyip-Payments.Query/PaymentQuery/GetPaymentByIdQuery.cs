using System.Threading;
using System.Threading.Tasks;
using Hyip_Payments.Models;
using Hyip_Payments.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    // Query to get a payment by its ID
    public class GetPaymentByIdQuery : IRequest<PaymentModel?>
    {
        public Guid Id { get; }
        public GetPaymentByIdQuery(int id) => Id = id;
    }

    // Handler for GetPaymentByIdQuery
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentModel?>
    {
        private readonly PaymentsDbContext _dbContext;

        public GetPaymentByIdQueryHandler(PaymentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaymentModel?> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        }
    }
}
