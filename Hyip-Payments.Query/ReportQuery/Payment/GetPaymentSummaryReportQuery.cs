using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hyip_Payments.Query.ReportQuery.Payment
{
    public class GetPaymentSummaryReportQuery : IRequest<PaymentSummaryReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetPaymentSummaryReportQueryHandler : IRequestHandler<GetPaymentSummaryReportQuery, PaymentSummaryReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentSummaryReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentSummaryReportModel> Handle(GetPaymentSummaryReportQuery request, CancellationToken cancellationToken)
        {
            // Get all payment transactions in date range
            var payments = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate)
                .Include(p => p.PaymentMethod)
                .ToListAsync(cancellationToken);

            // Payments by status
            var paymentsByStatus = payments
                .GroupBy(p => p.Status ?? "Unknown")
                .Select(g => new PaymentByStatusDto
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount)
                })
                .ToList();

            var totalPayments = payments.Count;
            var totalAmount = payments.Sum(p => p.Amount);

            // Calculate percentages and success rates
            foreach (var status in paymentsByStatus)
            {
                status.Percentage = totalPayments > 0 
                    ? (decimal)status.Count / totalPayments * 100 
                    : 0;
                
                // Success rate is 100% for Completed, 0% for Failed/Cancelled, and in-progress for Pending
                status.SuccessRate = status.Status switch
                {
                    "Completed" => 100m,
                    "Failed" => 0m,
                    "Cancelled" => 0m,
                    "Pending" => 50m, // Neutral for pending
                    _ => 0m
                };
            }

            // Payments by payment method
            var paymentsByMethod = payments
                .GroupBy(p => p.PaymentMethod?.Name ?? "Unknown")
                .Select(g => new PaymentByMethodDto
                {
                    PaymentMethodName = g.Key,
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount),
                    CompletedCount = g.Count(p => p.Status == "Completed"),
                    FailedCount = g.Count(p => p.Status == "Failed")
                })
                .ToList();

            // Calculate percentages for payment methods
            foreach (var method in paymentsByMethod)
            {
                method.Percentage = totalAmount > 0 
                    ? method.TotalAmount / totalAmount * 100 
                    : 0;
            }

            // Payments by month
            var paymentsByMonth = payments
                .GroupBy(p => new { p.TransactionDate.Year, p.TransactionDate.Month })
                .Select(g => new PaymentByMonthDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    PaymentCount = g.Count(),
                    TotalAmount = g.Sum(p => p.Amount),
                    CompletedCount = g.Count(p => p.Status == "Completed"),
                    FailedCount = g.Count(p => p.Status == "Failed")
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            // Status summary
            var completedCount = payments.Count(p => p.Status == "Completed");
            var statusSummary = new PaymentStatusSummaryDto
            {
                CompletedCount = completedCount,
                PendingCount = payments.Count(p => p.Status == "Pending"),
                FailedCount = payments.Count(p => p.Status == "Failed"),
                CancelledCount = payments.Count(p => p.Status == "Cancelled"),
                CompletedAmount = payments.Where(p => p.Status == "Completed").Sum(p => p.Amount),
                PendingAmount = payments.Where(p => p.Status == "Pending").Sum(p => p.Amount),
                FailedAmount = payments.Where(p => p.Status == "Failed").Sum(p => p.Amount),
                CancelledAmount = payments.Where(p => p.Status == "Cancelled").Sum(p => p.Amount),
                OverallSuccessRate = totalPayments > 0 ? (decimal)completedCount / totalPayments * 100 : 0
            };

            return new PaymentSummaryReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPayments = totalPayments,
                TotalPaymentAmount = totalAmount,
                AveragePaymentAmount = totalPayments > 0 ? totalAmount / totalPayments : 0,
                PaymentsByStatus = paymentsByStatus,
                PaymentsByMethod = paymentsByMethod,
                PaymentsByMonth = paymentsByMonth,
                StatusSummary = statusSummary
            };
        }
    }
}
