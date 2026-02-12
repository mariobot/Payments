using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    /// <summary>
    /// Query to get a payment transaction by ID
    /// </summary>
    public class GetPaymentTransactionByIdQuery : IRequest<PaymentTransactionModel?>
    {
        public int Id { get; }

        public GetPaymentTransactionByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetPaymentTransactionByIdQueryHandler : IRequestHandler<GetPaymentTransactionByIdQuery, PaymentTransactionModel?>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentTransactionByIdQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransactionModel?> Handle(GetPaymentTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentTransactions
                .Include(pt => pt.Wallet)
                .Include(pt => pt.PaymentMethod)
                .Include(pt => pt.Invoice)
                .AsNoTracking()
                .FirstOrDefaultAsync(pt => pt.Id == request.Id, cancellationToken);
        }
    }
}
