using Hyip_Payments.Context;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.PaymentQuery
{
    /// <summary>
    /// Query to get all payment transactions
    /// </summary>
    public class GetPaymentTransactionListQuery : IRequest<List<PaymentTransactionModel>>
    {
    }

    public class GetPaymentTransactionListQueryHandler : IRequestHandler<GetPaymentTransactionListQuery, List<PaymentTransactionModel>>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentTransactionListQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentTransactionModel>> Handle(GetPaymentTransactionListQuery request, CancellationToken cancellationToken)
        {
            return await _context.PaymentTransactions
                .Include(pt => pt.Wallet)
                .Include(pt => pt.PaymentMethod)
                .Include(pt => pt.Invoice)
                .AsNoTracking()
                .OrderByDescending(pt => pt.TransactionDate)
                .ToListAsync(cancellationToken);
        }
    }
}
