namespace Hyip_Payments.Models.Reports
{
    public class UserActivityReportModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public List<UserActivityDto> UserActivities { get; set; } = new();
        public UserActivitySummaryDto Summary { get; set; } = new();
    }

    public class UserActivityDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        // Activity Metrics
        public int InvoicesCreated { get; set; }
        public int PaymentsProcessed { get; set; }
        public int ProductsCreated { get; set; }
        public decimal TotalRevenueGenerated { get; set; }
        public int TotalActions { get; set; }
        
        // Engagement Score (0-100)
        public decimal EngagementScore { get; set; }
        public string EngagementLevel { get; set; } = string.Empty; // High, Medium, Low, Inactive
    }

    public class UserActivitySummaryDto
    {
        public int HighlyActiveUsers { get; set; }
        public int ModeratelyActiveUsers { get; set; }
        public int LowActivityUsers { get; set; }
        public int InactiveUserCount { get; set; }
        
        public string MostActiveUser { get; set; } = string.Empty;
        public int MostActiveUserActions { get; set; }
        
        public string TopRevenueUser { get; set; } = string.Empty;
        public decimal TopRevenueUserAmount { get; set; }
        
        public decimal AverageEngagementScore { get; set; }
        public decimal AverageActionsPerUser { get; set; }
        
        public Dictionary<string, int> ActionsByRole { get; set; } = new();
        public Dictionary<string, int> UsersByRole { get; set; } = new();
    }
}
