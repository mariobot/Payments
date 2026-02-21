namespace Hyip_Payments.Models.Reports
{
    public class ProfitLossReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Revenue Section
        public decimal TotalRevenue { get; set; }
        public int TotalRevenueTransactions { get; set; }
        
        // Expense Section
        public decimal TotalExpenses { get; set; }
        public int TotalExpenseTransactions { get; set; }
        
        // Profit/Loss Calculations
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public bool IsProfitable { get; set; }
        
        // Detailed Breakdowns
        public List<ProfitLossByDateDto> ProfitLossByDate { get; set; } = new();
        public ProfitLossSummaryDto Summary { get; set; } = new();
    }

    public class ProfitLossByDateDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
    }

    public class ProfitLossSummaryDto
    {
        // Revenue Breakdown
        public decimal CompletedPaymentsRevenue { get; set; }
        public int CompletedPaymentsCount { get; set; }
        
        // Expense Breakdown
        public decimal ProcessingFeesExpense { get; set; }
        public decimal FailedTransactionCosts { get; set; }
        public decimal OtherExpenses { get; set; }
        
        // Performance Metrics
        public decimal RevenueGrowth { get; set; }
        public decimal ExpenseGrowth { get; set; }
        public decimal AverageRevenuePerDay { get; set; }
        public decimal AverageExpensePerDay { get; set; }
        public decimal AverageProfitPerDay { get; set; }
        
        // Financial Health Indicators
        public string FinancialHealth { get; set; } = string.Empty; // Excellent, Good, Fair, Poor
        public decimal ExpenseRatio { get; set; } // Expenses as % of Revenue
        public int ProfitableDays { get; set; }
        public int UnprofitableDays { get; set; }
    }
}
