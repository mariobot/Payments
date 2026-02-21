namespace Hyip_Payments.Models.Reports
{
    public class DraftInvoicesReportModel
    {
        public DateTime ReportDate { get; set; }
        public int TotalDraftInvoices { get; set; }
        public decimal TotalDraftAmount { get; set; }
        public decimal AverageDraftAmount { get; set; }
        public List<DraftInvoiceDto> DraftInvoices { get; set; } = new();
        public DraftSummaryDto Summary { get; set; } = new();
    }

    public class DraftInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int DaysInDraft { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Description { get; set; }
        public int ItemCount { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class DraftSummaryDto
    {
        public int NewDrafts0To7Days { get; set; }
        public int Medium8To30Days { get; set; }
        public int OldOver30Days { get; set; }
        public decimal NewDraftsAmount { get; set; }
        public decimal MediumDraftsAmount { get; set; }
        public decimal OldDraftsAmount { get; set; }
        public int OldestDraftDays { get; set; }
        public decimal LargestDraftAmount { get; set; }
    }
}
