using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Financial
{
    public class GetRevenueReportQuery : IRequest<RevenueReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetRevenueReportQueryHandler : IRequestHandler<GetRevenueReportQuery, RevenueReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetRevenueReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<RevenueReportModel> Handle(GetRevenueReportQuery request, CancellationToken cancellationToken)
        {
            // Get completed payment transactions in date range
            var transactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate
                         && p.Status == "Completed")
                .Include(p => p.PaymentMethod)
                .ToListAsync(cancellationToken);

            // Revenue by date
            var revenueByDate = transactions
                .GroupBy(p => p.TransactionDate.Date)
                .Select(g => new RevenueByDateDto
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(r => r.Date)
                .ToList();

            // Revenue by payment method
            var revenueByMethod = transactions
                .GroupBy(p => p.PaymentMethod?.Name ?? "Unknown")
                .Select(g => new RevenueByMethodDto
                {
                    PaymentMethodName = g.Key,
                    TotalAmount = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var totalRevenue = transactions.Sum(p => p.Amount);

            // Calculate percentages
            foreach (var method in revenueByMethod)
            {
                method.Percentage = totalRevenue > 0 
                    ? (method.TotalAmount / totalRevenue) * 100 
                    : 0;
            }

            return new RevenueReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalRevenue = totalRevenue,
                TotalTransactions = transactions.Count,
                AverageTransactionValue = transactions.Count > 0 
                    ? totalRevenue / transactions.Count 
                    : 0,
                RevenueByDate = revenueByDate,
                RevenueByPaymentMethod = revenueByMethod
            };
        }
    }
}
