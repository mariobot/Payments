namespace Hyip_Payments.Models.Reports
{
    public class PaymentStatusReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PaymentStatusDto> PaymentsByStatus { get; set; } = new();
        public PaymentStatusBreakdownDto Summary { get; set; } = new();
    }

    public class PaymentStatusDto
    {
        public string Status { get; set; } = string.Empty; // Pending, Completed, Failed, Cancelled
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public decimal AverageAmount { get; set; }
        public List<PaymentDetailDto> RecentPayments { get; set; } = new();
    }

    public class PaymentDetailDto
    {
        public int PaymentId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
    }

    public class PaymentStatusBreakdownDto
    {
        // Status Counts
        public int PendingCount { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
        public int CancelledCount { get; set; }

        // Status Amounts
        public decimal PendingAmount { get; set; }
        public decimal CompletedAmount { get; set; }
        public decimal FailedAmount { get; set; }
        public decimal CancelledAmount { get; set; }

        // Success Metrics
        public decimal SuccessRate { get; set; }
        public decimal FailureRate { get; set; }
        public decimal CompletionRate { get; set; }

        // Financial Impact
        public decimal SuccessfulRevenue { get; set; }
        public decimal LostRevenue { get; set; } // Failed + Cancelled
        public decimal PendingRevenue { get; set; }

        // Processing Insights
        public string MostCommonStatus { get; set; } = string.Empty;
        public decimal AverageSuccessfulAmount { get; set; }
        public decimal AverageFailedAmount { get; set; }
    }
}
