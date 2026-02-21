using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.User
{
    public class GetUserActivityReportQuery : IRequest<UserActivityReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Role { get; set; }
    }

    public class GetUserActivityReportQueryHandler : IRequestHandler<GetUserActivityReportQuery, UserActivityReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetUserActivityReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserActivityReportModel> Handle(GetUserActivityReportQuery request, CancellationToken cancellationToken)
        {
            // Get all users
            var usersQuery = _context.Users.AsQueryable();

            // Role filtering not supported without Role property
            // Filtering removed for now

            var users = await usersQuery.ToListAsync(cancellationToken);

            // Get invoices created in date range
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= request.StartDate && i.InvoiceDate <= request.EndDate)
                .ToListAsync(cancellationToken);

            // Get payments created in date range
            var payments = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate && p.TransactionDate <= request.EndDate)
                .ToListAsync(cancellationToken);

            // Get products created in date range (if CreatedDate exists)
            var products = await _context.Products
                .ToListAsync(cancellationToken);

            // Calculate user activities
            var userActivities = users.Select(user =>
            {
                var userInvoices = invoices.Where(i => i.CreatedByUserId == user.Id.ToString()).ToList();
                var userPayments = payments.Where(p => p.ProcessedByUserId == user.Id.ToString()).ToList();
                // Products don't have CreatedBy field
                var productsCreated = 0;

                var invoicesCreated = userInvoices.Count;
                var paymentsProcessed = userPayments.Count;
                var totalActions = invoicesCreated + paymentsProcessed + productsCreated;

                var totalRevenue = userPayments
                    .Where(p => p.Status == "Completed")
                    .Sum(p => p.Amount);

                // Calculate engagement score (0-100)
                var engagementScore = CalculateEngagementScore(totalActions, totalRevenue, user.IsActive);

                // Determine engagement level
                string engagementLevel;
                if (!user.IsActive)
                    engagementLevel = "Inactive";
                else if (engagementScore >= 70)
                    engagementLevel = "High";
                else if (engagementScore >= 40)
                    engagementLevel = "Medium";
                else if (engagementScore >= 10)
                    engagementLevel = "Low";
                else
                    engagementLevel = "Minimal";

                return new UserActivityDto
                {
                    UserId = user.Id.ToString(),
                    Username = user.Username ?? "Unknown",
                    Email = user.Email ?? "N/A",
                    Role = "User", // Default role since Role property doesn't exist
                    IsActive = user.IsActive,
                    LastLoginDate = null, // Would need LastLoginDate field
                    InvoicesCreated = invoicesCreated,
                    PaymentsProcessed = paymentsProcessed,
                    ProductsCreated = productsCreated,
                    TotalRevenueGenerated = totalRevenue,
                    TotalActions = totalActions,
                    EngagementScore = engagementScore,
                    EngagementLevel = engagementLevel
                };
            })
            .OrderByDescending(u => u.EngagementScore)
            .ToList();

            // Calculate summary
            var activeUsers = userActivities.Count(u => u.IsActive);
            var inactiveUsers = userActivities.Count(u => !u.IsActive);

            var highlyActive = userActivities.Count(u => u.EngagementLevel == "High");
            var moderatelyActive = userActivities.Count(u => u.EngagementLevel == "Medium");
            var lowActivity = userActivities.Count(u => u.EngagementLevel == "Low");

            var mostActiveUser = userActivities
                .OrderByDescending(u => u.TotalActions)
                .FirstOrDefault();

            var topRevenueUser = userActivities
                .OrderByDescending(u => u.TotalRevenueGenerated)
                .FirstOrDefault();

            var actionsByRole = userActivities
                .GroupBy(u => u.Role)
                .ToDictionary(g => g.Key, g => g.Sum(u => u.TotalActions));

            var usersByRole = userActivities
                .GroupBy(u => u.Role)
                .ToDictionary(g => g.Key, g => g.Count());

            var summary = new UserActivitySummaryDto
            {
                HighlyActiveUsers = highlyActive,
                ModeratelyActiveUsers = moderatelyActive,
                LowActivityUsers = lowActivity,
                InactiveUserCount = inactiveUsers,
                MostActiveUser = mostActiveUser?.Username ?? "None",
                MostActiveUserActions = mostActiveUser?.TotalActions ?? 0,
                TopRevenueUser = topRevenueUser?.Username ?? "None",
                TopRevenueUserAmount = topRevenueUser?.TotalRevenueGenerated ?? 0,
                AverageEngagementScore = userActivities.Any() ? userActivities.Average(u => u.EngagementScore) : 0,
                AverageActionsPerUser = userActivities.Any() ? (decimal)userActivities.Average(u => u.TotalActions) : 0,
                ActionsByRole = actionsByRole,
                UsersByRole = usersByRole
            };

            return new UserActivityReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalUsers = users.Count,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                UserActivities = userActivities,
                Summary = summary
            };
        }

        private decimal CalculateEngagementScore(int totalActions, decimal totalRevenue, bool isActive)
        {
            if (!isActive)
                return 0;

            // Base score from actions (0-60 points)
            var actionScore = Math.Min(totalActions * 2, 60);

            // Revenue bonus (0-30 points)
            var revenueScore = Math.Min((int)(totalRevenue / 1000), 30);

            // Active status bonus (10 points)
            var activeBonus = 10;

            return actionScore + revenueScore + activeBonus;
        }
    }
}
