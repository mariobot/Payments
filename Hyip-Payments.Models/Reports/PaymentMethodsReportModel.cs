namespace Hyip_Payments.Models.Reports
{
    public class PaymentMethodsReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPaymentMethods { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PaymentMethodDetailDto> PaymentMethods { get; set; } = new();
        public PaymentMethodSummaryDto Summary { get; set; } = new();
    }

    public class PaymentMethodDetailDto
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; } = string.Empty;
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal SuccessRate { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public decimal LargestTransaction { get; set; }
        public decimal SmallestTransaction { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }

    public class PaymentMethodSummaryDto
    {
        public string MostUsedMethod { get; set; } = string.Empty;
        public string HighestRevenueMethod { get; set; } = string.Empty;
        public string BestSuccessRateMethod { get; set; } = string.Empty;
        public int ActiveMethods { get; set; }
        public int InactiveMethods { get; set; }
        public decimal AverageSuccessRate { get; set; }
        public decimal TotalSuccessfulAmount { get; set; }
        public decimal TotalFailedAmount { get; set; }
    }
}
