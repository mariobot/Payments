namespace Hyip_Payments.Models.Reports
{
    public class AuditTrailReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalEvents { get; set; }
        public List<AuditEventDto> AuditEvents { get; set; } = new();
        public AuditTrailSummaryDto Summary { get; set; } = new();
    }

    public class AuditEventDto
    {
        public int EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout, View, etc.
        public string EntityType { get; set; } = string.Empty; // Invoice, Payment, User, Product, etc.
        public string EntityId { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // Info, Warning, Critical
        public bool IsSuccessful { get; set; }
    }

    public class AuditTrailSummaryDto
    {
        public int CreateActions { get; set; }
        public int UpdateActions { get; set; }
        public int DeleteActions { get; set; }
        public int ViewActions { get; set; }
        public int LoginActions { get; set; }
        public int SecurityEvents { get; set; }
        
        public Dictionary<string, int> ActionsByType { get; set; } = new();
        public Dictionary<string, int> ActionsByEntity { get; set; } = new();
        public Dictionary<string, int> ActionsByUser { get; set; } = new();
        public Dictionary<string, int> ActionsBySeverity { get; set; } = new();
        
        public string MostActiveUser { get; set; } = string.Empty;
        public int MostActiveUserActions { get; set; }
        
        public string MostAuditedEntity { get; set; } = string.Empty;
        public int MostAuditedEntityCount { get; set; }
        
        public int CriticalEvents { get; set; }
        public int WarningEvents { get; set; }
        public int InfoEvents { get; set; }
    }
}
