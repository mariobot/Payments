namespace Hyip_Payments.Models.Reports
{
    public class PaidInvoicesReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPaidInvoices { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal AveragePaidAmount { get; set; }
        public List<PaidInvoiceDto> PaidInvoices { get; set; } = new();
        public PaidSummaryDto Summary { get; set; } = new();
    }

    public class PaidInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public int DaysToPay { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Description { get; set; }
        public string? PaymentMethod { get; set; }
        public int ItemCount { get; set; }
        public string ProcessedBy { get; set; } = string.Empty;
    }

    public class PaidSummaryDto
    {
        public int PaidWithin7Days { get; set; }
        public int Paid8To30Days { get; set; }
        public int PaidOver30Days { get; set; }
        public decimal PaidWithin7DaysAmount { get; set; }
        public decimal Paid8To30DaysAmount { get; set; }
        public decimal PaidOver30DaysAmount { get; set; }
        public decimal AverageDaysToPay { get; set; }
        public decimal LargestPayment { get; set; }
        public decimal SmallestPayment { get; set; }
    }
}
