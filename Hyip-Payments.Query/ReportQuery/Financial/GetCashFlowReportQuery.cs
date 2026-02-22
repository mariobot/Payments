using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Financial
{
    public class GetCashFlowReportQuery : IRequest<CashFlowReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetCashFlowReportQueryHandler : IRequestHandler<GetCashFlowReportQuery, CashFlowReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetCashFlowReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<CashFlowReportModel> Handle(GetCashFlowReportQuery request, CancellationToken cancellationToken)
        {
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date;

            // Get completed payments (cash inflows - operating activities)
            var completedPayments = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed" 
                         && p.TransactionDate >= startDate 
                         && p.TransactionDate <= endDate)
                .ToListAsync(cancellationToken);

            // Get failed/cancelled payments (potential cash outflows from refunds)
            var failedPayments = await _context.PaymentTransactions
                .Where(p => (p.Status == "Failed" || p.Status == "Cancelled")
                         && p.TransactionDate >= startDate 
                         && p.TransactionDate <= endDate)
                .ToListAsync(cancellationToken);

            // Calculate opening balance (sum of all completed payments before start date)
            var openingBalance = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed" && p.TransactionDate < startDate)
                .SumAsync(p => p.Amount, cancellationToken);

            // Cash Inflows (Operating Activities)
            var cashInflows = new List<CashFlowActivityDto>();

            // Group completed payments
            var completedGroups = completedPayments
                .GroupBy(p => p.TransactionDate.Date)
                .Select(g => new CashFlowActivityDto
                {
                    ActivityType = "Operating",
                    Description = "Payment Receipts",
                    Amount = g.Sum(p => p.Amount),
                    TransactionDate = g.Key,
                    Category = "Revenue",
                    TransactionCount = g.Count()
                })
                .ToList();

            cashInflows.AddRange(completedGroups);

            // Cash Outflows (Operating Activities)
            var cashOutflows = new List<CashFlowActivityDto>();

            // Processing fees (2% of completed payments)
            var processingFees = completedPayments
                .GroupBy(p => p.TransactionDate.Date)
                .Select(g => new CashFlowActivityDto
                {
                    ActivityType = "Operating",
                    Description = "Payment Processing Fees",
                    Amount = g.Sum(p => p.Amount) * 0.02m,
                    TransactionDate = g.Key,
                    Category = "Fees",
                    TransactionCount = g.Count()
                })
                .ToList();

            cashOutflows.AddRange(processingFees);

            // Failed transaction costs (2.5% of failed amounts)
            if (failedPayments.Any())
            {
                var failedCosts = failedPayments
                    .GroupBy(p => p.TransactionDate.Date)
                    .Select(g => new CashFlowActivityDto
                    {
                        ActivityType = "Operating",
                        Description = "Failed Transaction Costs",
                        Amount = g.Sum(p => p.Amount) * 0.025m,
                        TransactionDate = g.Key,
                        Category = "Operational Costs",
                        TransactionCount = g.Count()
                    })
                    .ToList();

                cashOutflows.AddRange(failedCosts);
            }

            // Calculate daily cash flow
            var allDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(d => startDate.AddDays(d))
                .ToList();

            var dailyCashFlow = new List<DailyCashFlowDto>();
            var runningBalance = openingBalance;

            foreach (var date in allDates)
            {
                var dayInflows = cashInflows
                    .Where(i => i.TransactionDate.Date == date)
                    .Sum(i => i.Amount);

                var dayOutflows = cashOutflows
                    .Where(o => o.TransactionDate.Date == date)
                    .Sum(o => o.Amount);

                var netFlow = dayInflows - dayOutflows;
                runningBalance += netFlow;

                dailyCashFlow.Add(new DailyCashFlowDto
                {
                    Date = date,
                    Inflows = dayInflows,
                    Outflows = dayOutflows,
                    NetFlow = netFlow,
                    RunningBalance = runningBalance
                });
            }

            var closingBalance = runningBalance;
            var netCashFlow = closingBalance - openingBalance;

            // Calculate summary metrics
            var totalInflows = cashInflows.Sum(i => i.Amount);
            var totalOutflows = cashOutflows.Sum(o => o.Amount);

            var operatingInflows = cashInflows
                .Where(i => i.ActivityType == "Operating")
                .Sum(i => i.Amount);

            var operatingOutflows = cashOutflows
                .Where(o => o.ActivityType == "Operating")
                .Sum(o => o.Amount);

            var investingInflows = cashInflows
                .Where(i => i.ActivityType == "Investing")
                .Sum(i => i.Amount);

            var investingOutflows = cashOutflows
                .Where(o => o.ActivityType == "Investing")
                .Sum(o => o.Amount);

            var financingInflows = cashInflows
                .Where(i => i.ActivityType == "Financing")
                .Sum(i => i.Amount);

            var financingOutflows = cashOutflows
                .Where(o => o.ActivityType == "Financing")
                .Sum(o => o.Amount);

            var operatingNetCashFlow = operatingInflows - operatingOutflows;
            var investingNetCashFlow = investingInflows - investingOutflows;
            var financingNetCashFlow = financingInflows - financingOutflows;

            var totalDays = dailyCashFlow.Count > 0 ? dailyCashFlow.Count : 1;
            var avgDailyInflow = totalInflows / totalDays;
            var avgDailyOutflow = totalOutflows / totalDays;
            var avgDailyNetFlow = netCashFlow / totalDays;

            var positiveDays = dailyCashFlow.Count(d => d.NetFlow > 0);
            var negativeDays = dailyCashFlow.Count(d => d.NetFlow < 0);

            var highestDailyInflow = dailyCashFlow.Any() ? dailyCashFlow.Max(d => d.Inflows) : 0;
            var highestDailyOutflow = dailyCashFlow.Any() ? dailyCashFlow.Max(d => d.Outflows) : 0;

            var cashFlowGrowthRate = openingBalance > 0 
                ? ((closingBalance - openingBalance) / openingBalance) * 100 
                : 0;

            var cashFlowHealth = netCashFlow >= 0 
                ? (netCashFlow > totalInflows * 0.2m ? "Positive" : "Stable")
                : "Negative";

            var summary = new CashFlowSummaryDto
            {
                TotalInflows = totalInflows,
                OperatingInflows = operatingInflows,
                InvestingInflows = investingInflows,
                FinancingInflows = financingInflows,
                TotalOutflows = totalOutflows,
                OperatingOutflows = operatingOutflows,
                InvestingOutflows = investingOutflows,
                FinancingOutflows = financingOutflows,
                OperatingNetCashFlow = operatingNetCashFlow,
                InvestingNetCashFlow = investingNetCashFlow,
                FinancingNetCashFlow = financingNetCashFlow,
                AverageDailyInflow = avgDailyInflow,
                AverageDailyOutflow = avgDailyOutflow,
                AverageDailyNetFlow = avgDailyNetFlow,
                CashFlowGrowthRate = cashFlowGrowthRate,
                CashFlowHealth = cashFlowHealth,
                PositiveDays = positiveDays,
                NegativeDays = negativeDays,
                HighestDailyInflow = highestDailyInflow,
                HighestDailyOutflow = highestDailyOutflow
            };

            return new CashFlowReportModel
            {
                StartDate = startDate,
                EndDate = endDate,
                OpeningBalance = openingBalance,
                ClosingBalance = closingBalance,
                NetCashFlow = netCashFlow,
                CashInflows = cashInflows.OrderByDescending(i => i.Amount).ToList(),
                CashOutflows = cashOutflows.OrderByDescending(o => o.Amount).ToList(),
                DailyCashFlow = dailyCashFlow,
                Summary = summary
            };
        }
    }
}
