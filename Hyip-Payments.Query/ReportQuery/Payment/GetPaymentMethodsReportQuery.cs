using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Payment
{
    public class GetPaymentMethodsReportQuery : IRequest<PaymentMethodsReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetPaymentMethodsReportQueryHandler : IRequestHandler<GetPaymentMethodsReportQuery, PaymentMethodsReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetPaymentMethodsReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodsReportModel> Handle(GetPaymentMethodsReportQuery request, CancellationToken cancellationToken)
        {
            // Get all payment transactions in date range with payment method
            var transactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate)
                .Include(p => p.PaymentMethod)
                .ToListAsync(cancellationToken);

            // Get all payment methods (active and inactive)
            var allPaymentMethods = await _context.PaymentMethods
                .ToListAsync(cancellationToken);

            // Analyze each payment method
            var paymentMethodDetails = allPaymentMethods.Select(method =>
            {
                var methodTransactions = transactions
                    .Where(t => t.PaymentMethodId == method.Id)
                    .ToList();

                var totalAmount = methodTransactions.Sum(t => t.Amount);
                var completedCount = methodTransactions.Count(t => t.Status == "Completed");
                var failedCount = methodTransactions.Count(t => t.Status == "Failed");
                var pendingCount = methodTransactions.Count(t => t.Status == "Pending");
                var cancelledCount = methodTransactions.Count(t => t.Status == "Cancelled");

                var successRate = methodTransactions.Count > 0
                    ? (decimal)completedCount / methodTransactions.Count * 100
                    : 0;

                return new PaymentMethodDetailDto
                {
                    PaymentMethodId = method.Id,
                    MethodName = method.Name ?? "Unknown",
                    TotalTransactions = methodTransactions.Count,
                    TotalAmount = totalAmount,
                    CompletedCount = completedCount,
                    FailedCount = failedCount,
                    PendingCount = pendingCount,
                    CancelledCount = cancelledCount,
                    SuccessRate = successRate,
                    AverageTransactionAmount = methodTransactions.Count > 0 
                        ? totalAmount / methodTransactions.Count 
                        : 0,
                    LargestTransaction = methodTransactions.Any() 
                        ? methodTransactions.Max(t => t.Amount) 
                        : 0,
                    SmallestTransaction = methodTransactions.Any() 
                        ? methodTransactions.Min(t => t.Amount) 
                        : 0,
                    IsActive = method.IsActive,
                    Description = method.Description
                };
            })
            .OrderByDescending(m => m.TotalAmount)
            .ToList();

            var totalAmount = paymentMethodDetails.Sum(m => m.TotalAmount);

            // Calculate percentages
            foreach (var method in paymentMethodDetails)
            {
                method.Percentage = totalAmount > 0 
                    ? method.TotalAmount / totalAmount * 100 
                    : 0;
            }

            // Create summary
            var methodsWithTransactions = paymentMethodDetails.Where(m => m.TotalTransactions > 0).ToList();
            
            var summary = new PaymentMethodSummaryDto
            {
                MostUsedMethod = methodsWithTransactions
                    .OrderByDescending(m => m.TotalTransactions)
                    .FirstOrDefault()?.MethodName ?? "None",
                HighestRevenueMethod = methodsWithTransactions
                    .OrderByDescending(m => m.TotalAmount)
                    .FirstOrDefault()?.MethodName ?? "None",
                BestSuccessRateMethod = methodsWithTransactions
                    .OrderByDescending(m => m.SuccessRate)
                    .FirstOrDefault()?.MethodName ?? "None",
                ActiveMethods = paymentMethodDetails.Count(m => m.IsActive),
                InactiveMethods = paymentMethodDetails.Count(m => !m.IsActive),
                AverageSuccessRate = methodsWithTransactions.Any() 
                    ? methodsWithTransactions.Average(m => m.SuccessRate) 
                    : 0,
                TotalSuccessfulAmount = transactions
                    .Where(t => t.Status == "Completed")
                    .Sum(t => t.Amount),
                TotalFailedAmount = transactions
                    .Where(t => t.Status == "Failed")
                    .Sum(t => t.Amount)
            };

            return new PaymentMethodsReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalPaymentMethods = paymentMethodDetails.Count,
                TotalTransactions = transactions.Count,
                TotalAmount = totalAmount,
                PaymentMethods = paymentMethodDetails,
                Summary = summary
            };
        }
    }
}
