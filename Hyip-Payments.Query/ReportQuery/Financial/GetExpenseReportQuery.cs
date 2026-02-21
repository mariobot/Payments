using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.Financial
{
    public class GetExpenseReportQuery : IRequest<ExpenseReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetExpenseReportQueryHandler : IRequestHandler<GetExpenseReportQuery, ExpenseReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetExpenseReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseReportModel> Handle(GetExpenseReportQuery request, CancellationToken cancellationToken)
        {
            // Simulate expenses based on failed and cancelled transactions
            // In a real system, you would have an Expenses table
            var failedTransactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate
                         && (p.Status == "Failed" || p.Status == "Cancelled"))
                .ToListAsync(cancellationToken);

            // Calculate expenses (assuming processing fees and failure costs)
            var expenses = failedTransactions.Select(t => new
            {
                Date = t.TransactionDate.Date,
                Amount = t.Amount * 0.025m, // 2.5% processing fee on failed transactions
                Category = t.Status == "Failed" ? "Failed Transaction Costs" : "Cancelled Transaction Fees",
                t.Id
            }).ToList();

            // Add processing fees for all completed transactions (simulated)
            var completedTransactions = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate 
                         && p.TransactionDate <= request.EndDate
                         && p.Status == "Completed")
                .ToListAsync(cancellationToken);

            var processingFees = completedTransactions.Select(t => new
            {
                Date = t.TransactionDate.Date,
                Amount = t.Amount * 0.02m, // 2% processing fee
                Category = "Processing Fees",
                t.Id
            }).ToList();

            // Combine all expenses
            var allExpenses = expenses.Concat(processingFees).ToList();

            // Group by date
            var expensesByDate = allExpenses
                .GroupBy(e => e.Date)
                .Select(g => new ExpenseByDateDto
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count()
                })
                .OrderBy(e => e.Date)
                .ToList();

            // Group by category
            var expensesByCategory = allExpenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseByCategoryDto
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount),
                    ExpenseCount = g.Count(),
                    AverageAmount = g.Average(e => e.Amount)
                })
                .OrderByDescending(e => e.TotalAmount)
                .ToList();

            var totalExpenses = allExpenses.Sum(e => e.Amount);

            // Calculate percentages
            foreach (var category in expensesByCategory)
            {
                category.Percentage = totalExpenses > 0 
                    ? category.TotalAmount / totalExpenses * 100 
                    : 0;
            }

            // Create summary
            var summary = new ExpenseSummaryDto
            {
                TransactionFeesExpense = expenses
                    .Where(e => e.Category == "Failed Transaction Costs")
                    .Sum(e => e.Amount),
                ProcessingFeesExpense = processingFees.Sum(e => e.Amount),
                FailedTransactionCosts = failedTransactions
                    .Where(t => t.Status == "Failed")
                    .Sum(t => t.Amount * 0.025m),
                OtherExpenses = expenses
                    .Where(e => e.Category == "Cancelled Transaction Fees")
                    .Sum(e => e.Amount),
                TransactionFeesCount = failedTransactions.Count(t => t.Status == "Failed"),
                ProcessingFeesCount = processingFees.Count,
                FailedTransactionsCount = failedTransactions.Count,
                LargestExpense = allExpenses.Any() ? allExpenses.Max(e => e.Amount) : 0,
                SmallestExpense = allExpenses.Any() ? allExpenses.Min(e => e.Amount) : 0,
                MedianExpense = allExpenses.Any() 
                    ? allExpenses.OrderBy(e => e.Amount).Skip(allExpenses.Count / 2).First().Amount 
                    : 0
            };

            return new ExpenseReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalExpenses = totalExpenses,
                TotalExpenseTransactions = allExpenses.Count,
                AverageExpenseAmount = allExpenses.Any() ? totalExpenses / allExpenses.Count : 0,
                ExpensesByDate = expensesByDate,
                ExpensesByCategory = expensesByCategory,
                Summary = summary
            };
        }
    }
}
