namespace Hyip_Payments.Models.Reports
{
    public class PaymentSummaryReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalPaymentAmount { get; set; }
        public decimal AveragePaymentAmount { get; set; }
        public List<PaymentByStatusDto> PaymentsByStatus { get; set; } = new();
        public List<PaymentByMethodDto> PaymentsByMethod { get; set; } = new();
        public List<PaymentByMonthDto> PaymentsByMonth { get; set; } = new();
        public PaymentStatusSummaryDto StatusSummary { get; set; } = new();
    }

    public class PaymentByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public decimal SuccessRate { get; set; }
    }

    public class PaymentByMethodDto
    {
        public string PaymentMethodName { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class PaymentByMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int PaymentCount { get; set; }
        public decimal TotalAmount { get; set; }
        public int CompletedCount { get; set; }
        public int FailedCount { get; set; }
    }

    public class PaymentStatusSummaryDto
    {
        public int CompletedCount { get; set; }
        public int PendingCount { get; set; }
        public int FailedCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal CompletedAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal FailedAmount { get; set; }
        public decimal CancelledAmount { get; set; }
        public decimal OverallSuccessRate { get; set; }
    }
}
