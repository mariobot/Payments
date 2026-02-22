using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.User
{
    public class GetUserRolesReportQuery : IRequest<UserRolesReportModel>
    {
        public DateTime ReportDate { get; set; }
    }

    public class GetUserRolesReportQueryHandler : IRequestHandler<GetUserRolesReportQuery, UserRolesReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetUserRolesReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<UserRolesReportModel> Handle(GetUserRolesReportQuery request, CancellationToken cancellationToken)
        {
            var reportDate = request.ReportDate.Date;

            // Get all users with their roles
            var users = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);

            // Get activity data
            var invoices = await _context.Invoices.ToListAsync(cancellationToken);
            var payments = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed")
                .ToListAsync(cancellationToken);
            var products = await _context.Products.ToListAsync(cancellationToken);

            var totalUsers = users.Count;

            // Get all unique roles from UserRoles
            var allRoles = users
                .SelectMany(u => u.UserRoles ?? new List<Models.UserRoleModel>())
                .Select(ur => ur.Role?.Name ?? "Unknown")
                .Distinct()
                .ToList();

            // If no roles found in UserRoles, use default roles
            if (!allRoles.Any())
            {
                allRoles = new List<string> { "Admin", "Manager", "User" };
            }

            var totalRoles = allRoles.Count;

            // Build role distribution
            var roleDistribution = allRoles.Select(roleName =>
            {
                var usersInRole = users
                    .Where(u => u.UserRoles?.Any(ur => ur.Role?.Name == roleName) ?? false)
                    .ToList();

                var userCount = usersInRole.Count;
                var activeUsers = usersInRole.Count(u => u.IsActive);
                var inactiveUsers = userCount - activeUsers;
                var percentage = totalUsers > 0 ? (decimal)userCount / totalUsers * 100 : 0;
                var activePercentage = userCount > 0 ? (decimal)activeUsers / userCount * 100 : 0;

                var usersDetails = usersInRole.Select(user =>
                {
                    var userInvoices = invoices.Where(i => i.CreatedByUserId == user.Id.ToString()).Count();
                    var userPayments = payments.Where(p => p.ProcessedByUserId == user.Id.ToString()).Count();
                    var userRevenue = payments
                        .Where(p => p.ProcessedByUserId == user.Id.ToString())
                        .Sum(p => p.Amount);

                    return new UserInRoleDto
                    {
                        UserId = user.Id,
                        Username = user.Username ?? "Unknown",
                        Email = user.Email ?? "N/A",
                        IsActive = user.IsActive,
                        TotalActions = userInvoices + userPayments,
                        TotalRevenue = userRevenue
                    };
                })
                .OrderByDescending(u => u.TotalActions)
                .ToList();

                return new UserRoleDto
                {
                    RoleName = roleName,
                    UserCount = userCount,
                    ActiveUsers = activeUsers,
                    InactiveUsers = inactiveUsers,
                    Percentage = percentage,
                    ActivePercentage = activePercentage,
                    Users = usersDetails
                };
            })
            .OrderByDescending(r => r.UserCount)
            .ToList();

            // Build role activities
            var roleActivities = allRoles.Select(roleName =>
            {
                var usersInRole = users
                    .Where(u => u.UserRoles?.Any(ur => ur.Role?.Name == roleName) ?? false)
                    .ToList();

                var totalInvoices = 0;
                var totalPayments = 0;
                var totalProducts = 0;
                var totalRevenue = 0m;

                foreach (var user in usersInRole)
                {
                    totalInvoices += invoices.Count(i => i.CreatedByUserId == user.Id.ToString());
                    totalPayments += payments.Count(p => p.ProcessedByUserId == user.Id.ToString());
                    totalRevenue += payments
                        .Where(p => p.ProcessedByUserId == user.Id.ToString())
                        .Sum(p => p.Amount);
                }

                var totalActions = totalInvoices + totalPayments + totalProducts;
                var avgRevenue = usersInRole.Count > 0 ? totalRevenue / usersInRole.Count : 0;

                return new RoleActivityDto
                {
                    RoleName = roleName,
                    TotalInvoices = totalInvoices,
                    TotalPayments = totalPayments,
                    TotalProducts = totalProducts,
                    TotalRevenue = totalRevenue,
                    AverageRevenuePerUser = avgRevenue,
                    TotalActions = totalActions,
                    ActivityPercentage = 0 // Will calculate after
                };
            })
            .ToList();

            // Calculate activity percentages
            var totalActivityActions = roleActivities.Sum(r => r.TotalActions);
            foreach (var activity in roleActivities)
            {
                activity.ActivityPercentage = totalActivityActions > 0 
                    ? (decimal)activity.TotalActions / totalActivityActions * 100 
                    : 0;
            }

            roleActivities = roleActivities.OrderByDescending(r => r.TotalActions).ToList();

            // Build summary
            var mostPopularRole = roleDistribution.OrderByDescending(r => r.UserCount).FirstOrDefault();
            var mostActiveRole = roleActivities.OrderByDescending(r => r.TotalActions).FirstOrDefault();
            var highestRevenueRole = roleActivities.OrderByDescending(r => r.TotalRevenue).FirstOrDefault();

            var usersWithRoles = users.Count(u => u.UserRoles != null && u.UserRoles.Any());
            var usersWithoutRoles = totalUsers - usersWithRoles;
            var usersWithMultipleRoles = users.Count(u => u.UserRoles != null && u.UserRoles.Count > 1);

            var avgActions = totalRoles > 0 ? (decimal)roleActivities.Sum(r => r.TotalActions) / totalRoles : 0;
            var avgUsers = totalRoles > 0 ? (decimal)roleDistribution.Sum(r => r.UserCount) / totalRoles : 0;
            var avgRevenue = totalRoles > 0 ? roleActivities.Sum(r => r.TotalRevenue) / totalRoles : 0;

            var topUserByRole = new Dictionary<string, string>();
            var actionsByRole = new Dictionary<string, int>();

            foreach (var role in roleDistribution)
            {
                var topUser = role.Users.OrderByDescending(u => u.TotalActions).FirstOrDefault();
                if (topUser != null)
                {
                    topUserByRole[role.RoleName] = topUser.Username;
                }

                var roleActivity = roleActivities.FirstOrDefault(r => r.RoleName == role.RoleName);
                if (roleActivity != null)
                {
                    actionsByRole[role.RoleName] = roleActivity.TotalActions;
                }
            }

            var summary = new UserRolesSummaryDto
            {
                MostPopularRole = mostPopularRole?.RoleName ?? "None",
                MostPopularRoleCount = mostPopularRole?.UserCount ?? 0,
                MostActiveRole = mostActiveRole?.RoleName ?? "None",
                MostActiveRoleActions = mostActiveRole?.TotalActions ?? 0,
                HighestRevenueRole = highestRevenueRole?.RoleName ?? "None",
                HighestRevenueRoleAmount = highestRevenueRole?.TotalRevenue ?? 0,
                UsersWithRoles = usersWithRoles,
                UsersWithoutRoles = usersWithoutRoles,
                UsersWithMultipleRoles = usersWithMultipleRoles,
                AverageActionsPerRole = avgActions,
                AverageUsersPerRole = avgUsers,
                AverageRevenuePerRole = avgRevenue,
                TopUserByRole = topUserByRole,
                ActionsByRole = actionsByRole
            };

            return new UserRolesReportModel
            {
                ReportDate = reportDate,
                TotalUsers = totalUsers,
                TotalRoles = totalRoles,
                RoleDistribution = roleDistribution,
                RoleActivities = roleActivities,
                Summary = summary
            };
        }
    }
}
