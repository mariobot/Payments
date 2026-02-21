namespace Hyip_Payments.Models.Reports
{
    public class InvoiceSummaryReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalInvoiceValue { get; set; }
        public decimal AverageInvoiceAmount { get; set; }
        public List<InvoiceByStatusDto> InvoicesByStatus { get; set; } = new();
        public List<InvoiceByMonthDto> InvoicesByMonth { get; set; } = new();
        public InvoiceStatusSummaryDto StatusSummary { get; set; } = new();
    }

    public class InvoiceByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class InvoiceByMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class InvoiceStatusSummaryDto
    {
        public int DraftCount { get; set; }
        public int PendingCount { get; set; }
        public int PaidCount { get; set; }
        public int CancelledCount { get; set; }
        public decimal DraftAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal CancelledAmount { get; set; }
    }
}
