namespace Hyip_Payments.Models
{
    public class CustomReportModel
    {
        public int Id { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty; // Financial, Invoice, Payment, User, Product, Custom
        public string DataSource { get; set; } = string.Empty; // Invoices, Payments, Users, Products
        
        // Configuration stored as JSON - contains the entire ReportConfiguration
        public string ConfigurationJson { get; set; } = string.Empty;
        
        // User & Sharing
        public string CreatedByUserId { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        
        // Metadata
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastRunDate { get; set; }
        public int RunCount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
