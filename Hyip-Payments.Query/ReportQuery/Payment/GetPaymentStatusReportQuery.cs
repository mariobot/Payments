using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Payment
{
    public class GetPaymentStatusReportQuery : IRequest<PaymentStatusReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? StatusFilter { get; set; }
    }

    public class GetPaymentStatusReportQueryHandler : IRequestHandler<GetPaymentStatusReportQuery, PaymentStatusReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentStatusReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentStatusReportModel> Handle(GetPaymentStatusReportQuery request, CancellationToken cancellationToken)
        {
            // Get all payment transactions in date range
            var query = _context.PaymentTransactions
                .Include(p => p.PaymentMethod)
                .Include(p => p.Invoice)
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate);

            // Apply status filter if provided
            if (!string.IsNullOrEmpty(request.StatusFilter))
            {
                query = query.Where(p => p.Status == request.StatusFilter);
            }

            var allPayments = await query.ToListAsync(cancellationToken);

            var totalPayments = allPayments.Count;
            var totalAmount = allPayments.Sum(p => p.Amount);

            // Group by status
            var statusGroups = allPayments
                .GroupBy(p => p.Status ?? "Unknown")
                .Select(g => new
                {
                    Status = g.Key,
                    Payments = g.ToList()
                })
                .ToList();

            var paymentsByStatus = statusGroups.Select(g =>
            {
                var count = g.Payments.Count;
                var amount = g.Payments.Sum(p => p.Amount);
                var percentage = totalPayments > 0 ? (decimal)count / totalPayments * 100 : 0;
                var avgAmount = count > 0 ? amount / count : 0;

                // Get recent payments for this status (top 10)
                var recentPayments = g.Payments
                    .OrderByDescending(p => p.TransactionDate)
                    .Take(10)
                    .Select(p => new PaymentDetailDto
                    {
                        PaymentId = p.Id,
                        TransactionDate = p.TransactionDate,
                        Amount = p.Amount,
                        Status = p.Status ?? "Unknown",
                        PaymentMethod = p.PaymentMethod?.Name ?? "Unknown",
                        InvoiceNumber = p.Invoice?.InvoiceNumber,
                        Reference = p.Reference,
                        Description = p.Description
                    })
                    .ToList();

                return new PaymentStatusDto
                {
                    Status = g.Status,
                    Count = count,
                    TotalAmount = amount,
                    Percentage = percentage,
                    AverageAmount = avgAmount,
                    RecentPayments = recentPayments
                };
            })
            .OrderByDescending(s => s.Count)
            .ToList();

            // Calculate summary metrics
            var pendingPayments = allPayments.Where(p => p.Status == "Pending").ToList();
            var completedPayments = allPayments.Where(p => p.Status == "Completed").ToList();
            var failedPayments = allPayments.Where(p => p.Status == "Failed").ToList();
            var cancelledPayments = allPayments.Where(p => p.Status == "Cancelled").ToList();

            var pendingCount = pendingPayments.Count;
            var completedCount = completedPayments.Count;
            var failedCount = failedPayments.Count;
            var cancelledCount = cancelledPayments.Count;

            var pendingAmount = pendingPayments.Sum(p => p.Amount);
            var completedAmount = completedPayments.Sum(p => p.Amount);
            var failedAmount = failedPayments.Sum(p => p.Amount);
            var cancelledAmount = cancelledPayments.Sum(p => p.Amount);

            var processedCount = completedCount + failedCount + cancelledCount;
            var successRate = processedCount > 0 ? (decimal)completedCount / processedCount * 100 : 0;
            var failureRate = processedCount > 0 ? (decimal)(failedCount + cancelledCount) / processedCount * 100 : 0;
            var completionRate = totalPayments > 0 ? (decimal)completedCount / totalPayments * 100 : 0;

            var lostRevenue = failedAmount + cancelledAmount;
            var avgSuccessful = completedCount > 0 ? completedAmount / completedCount : 0;
            var avgFailed = failedCount > 0 ? failedAmount / failedCount : 0;

            var mostCommonStatus = paymentsByStatus.OrderByDescending(s => s.Count).FirstOrDefault()?.Status ?? "None";

            var summary = new PaymentStatusBreakdownDto
            {
                PendingCount = pendingCount,
                CompletedCount = completedCount,
                FailedCount = failedCount,
                CancelledCount = cancelledCount,
                PendingAmount = pendingAmount,
                CompletedAmount = completedAmount,
                FailedAmount = failedAmount,
                CancelledAmount = cancelledAmount,
                SuccessRate = successRate,
                FailureRate = failureRate,
                CompletionRate = completionRate,
                SuccessfulRevenue = completedAmount,
                LostRevenue = lostRevenue,
                PendingRevenue = pendingAmount,
                MostCommonStatus = mostCommonStatus,
                AverageSuccessfulAmount = avgSuccessful,
                AverageFailedAmount = avgFailed
            };

            return new PaymentStatusReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPayments = totalPayments,
                TotalAmount = totalAmount,
                PaymentsByStatus = paymentsByStatus,
                Summary = summary
            };
        }
    }
}
