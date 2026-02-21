namespace Hyip_Payments.Models.Reports
{
    public class InvoiceAgingReportModel
    {
        public DateTime ReportDate { get; set; }
        public int TotalUnpaidInvoices { get; set; }
        public decimal TotalUnpaidAmount { get; set; }
        public List<InvoiceAgingBucketDto> AgingBuckets { get; set; } = new();
        public List<InvoiceAgingDetailDto> AgingDetails { get; set; } = new();
        public AgingSummaryDto Summary { get; set; } = new();
    }

    public class InvoiceAgingBucketDto
    {
        public string BucketName { get; set; } = string.Empty;
        public int MinDays { get; set; }
        public int? MaxDays { get; set; }
        public int InvoiceCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Percentage { get; set; }
        public string RiskLevel { get; set; } = string.Empty; // Low, Medium, High, Critical
    }

    public class InvoiceAgingDetailDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int DaysOld { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AgingBucket { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
    }

    public class AgingSummaryDto
    {
        public int Current0To30Count { get; set; }
        public int Days31To60Count { get; set; }
        public int Days61To90Count { get; set; }
        public int Over90DaysCount { get; set; }
        public decimal Current0To30Amount { get; set; }
        public decimal Days31To60Amount { get; set; }
        public decimal Days61To90Amount { get; set; }
        public decimal Over90DaysAmount { get; set; }
        public decimal AverageAgeDays { get; set; }
        public decimal OldestInvoiceDays { get; set; }
    }
}
