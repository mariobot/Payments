namespace Hyip_Payments.Models.Reports
{
    public class TransactionLogReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public List<TransactionLogDto> Transactions { get; set; } = new();
        public TransactionLogSummaryDto Summary { get; set; } = new();
    }

    public class TransactionLogDto
    {
        public int TransactionId { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? InvoiceNumber { get; set; }
        public int? InvoiceId { get; set; }
        public string? Description { get; set; }
        public string? ProcessedBy { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }

    public class TransactionLogSummaryDto
    {
        public int CompletedTransactions { get; set; }
        public int PendingTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public int CancelledTransactions { get; set; }
        public decimal CompletedAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal FailedAmount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public Dictionary<string, int> TransactionsByMethod { get; set; } = new();
        public Dictionary<string, int> TransactionsByHour { get; set; } = new();
    }
}
