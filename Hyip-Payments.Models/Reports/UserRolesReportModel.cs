namespace Hyip_Payments.Models.Reports
{
    public class UserRolesReportModel
    {
        public DateTime ReportDate { get; set; }
        public int TotalUsers { get; set; }
        public int TotalRoles { get; set; }
        public List<UserRoleDto> RoleDistribution { get; set; } = new();
        public List<RoleActivityDto> RoleActivities { get; set; } = new();
        public UserRolesSummaryDto Summary { get; set; } = new();
    }

    public class UserRoleDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public decimal Percentage { get; set; }
        public decimal ActivePercentage { get; set; }
        public List<UserInRoleDto> Users { get; set; } = new();
    }

    public class UserInRoleDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int TotalActions { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RoleActivityDto
    {
        public string RoleName { get; set; } = string.Empty;
        public int TotalInvoices { get; set; }
        public int TotalPayments { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageRevenuePerUser { get; set; }
        public int TotalActions { get; set; }
        public decimal ActivityPercentage { get; set; }
    }

    public class UserRolesSummaryDto
    {
        // Role Statistics
        public string MostPopularRole { get; set; } = string.Empty;
        public int MostPopularRoleCount { get; set; }
        public string MostActiveRole { get; set; } = string.Empty;
        public int MostActiveRoleActions { get; set; }
        public string HighestRevenueRole { get; set; } = string.Empty;
        public decimal HighestRevenueRoleAmount { get; set; }
        
        // User Distribution
        public int UsersWithRoles { get; set; }
        public int UsersWithoutRoles { get; set; }
        public int UsersWithMultipleRoles { get; set; }
        
        // Activity Metrics
        public decimal AverageActionsPerRole { get; set; }
        public decimal AverageUsersPerRole { get; set; }
        public decimal AverageRevenuePerRole { get; set; }
        
        // Top Performers by Role
        public Dictionary<string, string> TopUserByRole { get; set; } = new();
        public Dictionary<string, int> ActionsByRole { get; set; } = new();
    }
}
