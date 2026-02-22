using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.User
{
    public class GetActiveUsersReportQuery : IRequest<ActiveUsersReportModel>
    {
        public DateTime ReportDate { get; set; }
    }

    public class GetActiveUsersReportQueryHandler : IRequestHandler<GetActiveUsersReportQuery, ActiveUsersReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetActiveUsersReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<ActiveUsersReportModel> Handle(GetActiveUsersReportQuery request, CancellationToken cancellationToken)
        {
            var reportDate = request.ReportDate.Date;
            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);
            var monthAgo = today.AddMonths(-1);

            // Get all users
            var allUsers = await _context.Users.ToListAsync(cancellationToken);

            // Get invoices for activity tracking
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= monthAgo)
                .ToListAsync(cancellationToken);

            // Get payments for activity tracking
            var payments = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= monthAgo)
                .ToListAsync(cancellationToken);

            var totalUsers = allUsers.Count;
            var activeUsers = allUsers.Count(u => u.IsActive);
            var inactiveUsers = totalUsers - activeUsers;
            var activePercentage = totalUsers > 0 ? (decimal)activeUsers / totalUsers * 100 : 0;

            // Build active users list
            var activeUsersList = allUsers
                .Where(u => u.IsActive)
                .Select(user =>
                {
                    var userInvoices = invoices.Where(i => i.CreatedByUserId == user.Id.ToString()).ToList();
                    var userPayments = payments.Where(p => p.ProcessedByUserId == user.Id.ToString()).ToList();

                    var totalInvoices = userInvoices.Count;
                    var totalPayments = userPayments.Count;
                    var totalRevenue = userPayments.Where(p => p.Status == "Completed").Sum(p => p.Amount);

                    // Simulate last login (would come from actual login tracking)
                    DateTime? lastLogin = null;
                    var daysSinceLogin = 0;
                    var activityStatus = "Dormant";

                    // Determine activity based on recent actions
                    var lastInvoiceDate = userInvoices.OrderByDescending(i => i.InvoiceDate).FirstOrDefault()?.InvoiceDate;
                    var lastPaymentDate = userPayments.OrderByDescending(p => p.TransactionDate).FirstOrDefault()?.TransactionDate;

                    var mostRecentActivity = new[] { lastInvoiceDate, lastPaymentDate }
                        .Where(d => d.HasValue)
                        .OrderByDescending(d => d)
                        .FirstOrDefault();

                    if (mostRecentActivity.HasValue)
                    {
                        lastLogin = mostRecentActivity.Value;
                        daysSinceLogin = (today - lastLogin.Value.Date).Days;

                        if (daysSinceLogin == 0)
                            activityStatus = "Active Today";
                        else if (daysSinceLogin <= 7)
                            activityStatus = "Active This Week";
                        else if (daysSinceLogin <= 30)
                            activityStatus = "Active This Month";
                        else
                            activityStatus = "Dormant";
                    }

                    return new ActiveUserDto
                    {
                        UserId = user.Id,
                        Username = user.Username ?? "Unknown",
                        Email = user.Email ?? "N/A",
                        IsActive = user.IsActive,
                        LastLoginDate = lastLogin,
                        DaysSinceLastLogin = daysSinceLogin,
                        TotalInvoices = totalInvoices,
                        TotalPayments = totalPayments,
                        TotalRevenue = totalRevenue,
                        ActivityStatus = activityStatus
                    };
                })
                .OrderBy(u => u.DaysSinceLastLogin)
                .ToList();

            // Build inactive users list
            var inactiveUsersList = allUsers
                .Where(u => !u.IsActive)
                .Select(user =>
                {
                    var userInvoices = invoices.Where(i => i.CreatedByUserId == user.Id.ToString()).ToList();
                    var userPayments = payments.Where(p => p.ProcessedByUserId == user.Id.ToString()).ToList();

                    var lastInvoiceDate = userInvoices.OrderByDescending(i => i.InvoiceDate).FirstOrDefault()?.InvoiceDate;
                    var lastPaymentDate = userPayments.OrderByDescending(p => p.TransactionDate).FirstOrDefault()?.TransactionDate;

                    var mostRecentActivity = new[] { lastInvoiceDate, lastPaymentDate }
                        .Where(d => d.HasValue)
                        .OrderByDescending(d => d)
                        .FirstOrDefault();

                    DateTime? lastLogin = mostRecentActivity;
                    var daysSinceLogin = lastLogin.HasValue ? (today - lastLogin.Value.Date).Days : 999;

                    var inactiveReason = "Disabled";
                    if (!lastLogin.HasValue)
                        inactiveReason = "Never Logged In";
                    else if (daysSinceLogin > 90)
                        inactiveReason = "Long Term Dormant";

                    return new InactiveUserDto
                    {
                        UserId = user.Id,
                        Username = user.Username ?? "Unknown",
                        Email = user.Email ?? "N/A",
                        LastLoginDate = lastLogin,
                        DaysSinceLastLogin = daysSinceLogin,
                        InactiveReason = inactiveReason
                    };
                })
                .OrderByDescending(u => u.DaysSinceLastLogin)
                .ToList();

            // Calculate summary metrics
            var activeToday = activeUsersList.Count(u => u.ActivityStatus == "Active Today");
            var activeThisWeek = activeUsersList.Count(u => u.ActivityStatus == "Active This Week" || u.ActivityStatus == "Active Today");
            var activeThisMonth = activeUsersList.Count(u => u.ActivityStatus == "Active This Month" || u.ActivityStatus == "Active This Week" || u.ActivityStatus == "Active Today");
            var dormantUsers = activeUsersList.Count(u => u.ActivityStatus == "Dormant");

            var avgInvoices = activeUsersList.Any() ? (decimal)activeUsersList.Sum(u => u.TotalInvoices) / activeUsers : 0;
            var avgPayments = activeUsersList.Any() ? (decimal)activeUsersList.Sum(u => u.TotalPayments) / activeUsers : 0;
            var avgRevenue = activeUsersList.Any() ? activeUsersList.Sum(u => u.TotalRevenue) / activeUsers : 0;

            var mostActive = activeUsersList
                .OrderByDescending(u => u.TotalInvoices + u.TotalPayments)
                .FirstOrDefault();

            var neverLoggedIn = inactiveUsersList.Count(u => u.InactiveReason == "Never Logged In");

            var summary = new ActiveUsersSummaryDto
            {
                ActiveToday = activeToday,
                ActiveThisWeek = activeThisWeek,
                ActiveThisMonth = activeThisMonth,
                DormantUsers = dormantUsers,
                AverageInvoicesPerActiveUser = avgInvoices,
                AveragePaymentsPerActiveUser = avgPayments,
                AverageRevenuePerActiveUser = avgRevenue,
                MostActiveUser = mostActive?.Username ?? "None",
                MostActiveUserActions = mostActive != null ? mostActive.TotalInvoices + mostActive.TotalPayments : 0,
                NewUsersThisMonth = 0, // Would need CreatedDate field
                UsersNeverLoggedIn = neverLoggedIn,
                WeekOverWeekGrowth = 0, // Would need historical data
                MonthOverMonthGrowth = 0 // Would need historical data
            };

            return new ActiveUsersReportModel
            {
                ReportDate = reportDate,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers,
                ActivePercentage = activePercentage,
                ActiveUsersList = activeUsersList,
                InactiveUsersList = inactiveUsersList,
                Summary = summary
            };
        }
    }
}
