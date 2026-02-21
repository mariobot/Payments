using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Financial
{
    public class GetProfitLossReportQuery : IRequest<ProfitLossReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetProfitLossReportQueryHandler : IRequestHandler<GetProfitLossReportQuery, ProfitLossReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetProfitLossReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<ProfitLossReportModel> Handle(GetProfitLossReportQuery request, CancellationToken cancellationToken)
        {
            // Get Revenue Data (Completed Transactions)
            var completedTransactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate
                         && p.Status == "Completed")
                .ToListAsync(cancellationToken);

            var totalRevenue = completedTransactions.Sum(t => t.Amount);

            // Get Expense Data (Failed/Cancelled + Processing Fees)
            var failedTransactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate
                         && (p.Status == "Failed" || p.Status == "Cancelled"))
                .ToListAsync(cancellationToken);

            // Calculate Expenses
            var processingFees = completedTransactions.Sum(t => t.Amount * 0.02m); // 2% processing fee
            var failedCosts = failedTransactions.Sum(t => t.Amount * 0.025m); // 2.5% failed cost
            var totalExpenses = processingFees + failedCosts;

            // Calculate Profit/Loss
            var grossProfit = totalRevenue;
            var netProfit = totalRevenue - totalExpenses;
            var profitMargin = totalRevenue > 0 ? (netProfit / totalRevenue * 100) : 0;

            // Group by date for daily P&L
            var allDates = completedTransactions
                .Select(t => t.TransactionDate.Date)
                .Union(failedTransactions.Select(t => t.TransactionDate.Date))
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var profitLossByDate = allDates.Select(date =>
            {
                var dateRevenue = completedTransactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => t.Amount);

                var dateCompletedForFees = completedTransactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => t.Amount * 0.02m);

                var dateFailedCosts = failedTransactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => t.Amount * 0.025m);

                var dateExpenses = dateCompletedForFees + dateFailedCosts;
                var dateNetProfit = dateRevenue - dateExpenses;
                var dateProfitMargin = dateRevenue > 0 ? (dateNetProfit / dateRevenue * 100) : 0;

                return new ProfitLossByDateDto
                {
                    Date = date,
                    Revenue = dateRevenue,
                    Expenses = dateExpenses,
                    NetProfit = dateNetProfit,
                    ProfitMargin = dateProfitMargin
                };
            }).ToList();

            // Calculate summary metrics
            var daysInPeriod = (request.EndDate - request.StartDate).Days + 1;
            var expenseRatio = totalRevenue > 0 ? (totalExpenses / totalRevenue * 100) : 0;

            var profitableDays = profitLossByDate.Count(d => d.NetProfit > 0);
            var unprofitableDays = profitLossByDate.Count(d => d.NetProfit < 0);

            // Determine financial health
            string financialHealth;
            if (profitMargin >= 30)
                financialHealth = "Excellent";
            else if (profitMargin >= 15)
                financialHealth = "Good";
            else if (profitMargin >= 5)
                financialHealth = "Fair";
            else if (profitMargin >= 0)
                financialHealth = "Poor";
            else
                financialHealth = "Loss";

            var summary = new ProfitLossSummaryDto
            {
                CompletedPaymentsRevenue = totalRevenue,
                CompletedPaymentsCount = completedTransactions.Count,
                ProcessingFeesExpense = processingFees,
                FailedTransactionCosts = failedCosts,
                OtherExpenses = 0,
                RevenueGrowth = 0, // Would need historical data
                ExpenseGrowth = 0, // Would need historical data
                AverageRevenuePerDay = daysInPeriod > 0 ? totalRevenue / daysInPeriod : 0,
                AverageExpensePerDay = daysInPeriod > 0 ? totalExpenses / daysInPeriod : 0,
                AverageProfitPerDay = daysInPeriod > 0 ? netProfit / daysInPeriod : 0,
                FinancialHealth = financialHealth,
                ExpenseRatio = expenseRatio,
                ProfitableDays = profitableDays,
                UnprofitableDays = unprofitableDays
            };

            return new ProfitLossReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalRevenue = totalRevenue,
                TotalRevenueTransactions = completedTransactions.Count,
                TotalExpenses = totalExpenses,
                TotalExpenseTransactions = failedTransactions.Count + completedTransactions.Count, // All transactions incur some cost
                GrossProfit = grossProfit,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,
                IsProfitable = netProfit > 0,
                ProfitLossByDate = profitLossByDate,
                Summary = summary
            };
        }
    }
}
