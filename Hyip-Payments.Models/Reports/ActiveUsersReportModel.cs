namespace Hyip_Payments.Models.Reports
{
    public class ActiveUsersReportModel
    {
        public DateTime ReportDate { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public decimal ActivePercentage { get; set; }
        public List<ActiveUserDto> ActiveUsersList { get; set; } = new();
        public List<InactiveUserDto> InactiveUsersList { get; set; } = new();
        public ActiveUsersSummaryDto Summary { get; set; } = new();
    }

    public class ActiveUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int DaysSinceLastLogin { get; set; }
        public int TotalInvoices { get; set; }
        public int TotalPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public string ActivityStatus { get; set; } = string.Empty; // Active Today, This Week, This Month, Dormant
    }

    public class InactiveUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LastLoginDate { get; set; }
        public int DaysSinceLastLogin { get; set; }
        public string InactiveReason { get; set; } = string.Empty; // Never Logged In, Dormant, Disabled
    }

    public class ActiveUsersSummaryDto
    {
        // Activity Breakdown
        public int ActiveToday { get; set; }
        public int ActiveThisWeek { get; set; }
        public int ActiveThisMonth { get; set; }
        public int DormantUsers { get; set; }
        
        // Engagement Metrics
        public decimal AverageInvoicesPerActiveUser { get; set; }
        public decimal AveragePaymentsPerActiveUser { get; set; }
        public decimal AverageRevenuePerActiveUser { get; set; }
        
        // Activity Insights
        public string MostActiveUser { get; set; } = string.Empty;
        public int MostActiveUserActions { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int UsersNeverLoggedIn { get; set; }
        
        // Trends
        public decimal WeekOverWeekGrowth { get; set; }
        public decimal MonthOverMonthGrowth { get; set; }
    }
}
