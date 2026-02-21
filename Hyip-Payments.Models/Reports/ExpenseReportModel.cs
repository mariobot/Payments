namespace Hyip_Payments.Models.Reports
{
    public class ExpenseReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalExpenses { get; set; }
        public int TotalExpenseTransactions { get; set; }
        public decimal AverageExpenseAmount { get; set; }
        public List<ExpenseByDateDto> ExpensesByDate { get; set; } = new();
        public List<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new();
        public ExpenseSummaryDto Summary { get; set; } = new();
    }

    public class ExpenseByDateDto
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
    }

    public class ExpenseByCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int ExpenseCount { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageAmount { get; set; }
    }

    public class ExpenseSummaryDto
    {
        public decimal TransactionFeesExpense { get; set; }
        public decimal ProcessingFeesExpense { get; set; }
        public decimal FailedTransactionCosts { get; set; }
        public decimal OtherExpenses { get; set; }
        public int TransactionFeesCount { get; set; }
        public int ProcessingFeesCount { get; set; }
        public int FailedTransactionsCount { get; set; }
        public decimal LargestExpense { get; set; }
        public decimal SmallestExpense { get; set; }
        public decimal MedianExpense { get; set; }
    }
}
