namespace Hyip_Payments.Models.Reports
{
    public class CashFlowReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal NetCashFlow { get; set; }
        public List<CashFlowActivityDto> CashInflows { get; set; } = new();
        public List<CashFlowActivityDto> CashOutflows { get; set; } = new();
        public List<DailyCashFlowDto> DailyCashFlow { get; set; } = new();
        public CashFlowSummaryDto Summary { get; set; } = new();
    }

    public class CashFlowActivityDto
    {
        public string ActivityType { get; set; } = string.Empty; // Operating, Investing, Financing
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
    }

    public class DailyCashFlowDto
    {
        public DateTime Date { get; set; }
        public decimal Inflows { get; set; }
        public decimal Outflows { get; set; }
        public decimal NetFlow { get; set; }
        public decimal RunningBalance { get; set; }
    }

    public class CashFlowSummaryDto
    {
        // Inflows
        public decimal TotalInflows { get; set; }
        public decimal OperatingInflows { get; set; }
        public decimal InvestingInflows { get; set; }
        public decimal FinancingInflows { get; set; }

        // Outflows
        public decimal TotalOutflows { get; set; }
        public decimal OperatingOutflows { get; set; }
        public decimal InvestingOutflows { get; set; }
        public decimal FinancingOutflows { get; set; }

        // Net Cash Flow by Activity
        public decimal OperatingNetCashFlow { get; set; }
        public decimal InvestingNetCashFlow { get; set; }
        public decimal FinancingNetCashFlow { get; set; }

        // Performance Metrics
        public decimal AverageDailyInflow { get; set; }
        public decimal AverageDailyOutflow { get; set; }
        public decimal AverageDailyNetFlow { get; set; }
        public decimal CashFlowGrowthRate { get; set; }

        // Health Indicators
        public string CashFlowHealth { get; set; } = string.Empty; // Positive, Negative, Stable
        public int PositiveDays { get; set; }
        public int NegativeDays { get; set; }
        public decimal HighestDailyInflow { get; set; }
        public decimal HighestDailyOutflow { get; set; }
    }
}
