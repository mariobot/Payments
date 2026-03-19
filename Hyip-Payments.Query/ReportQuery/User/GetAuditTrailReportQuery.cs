using Hyip_Payments.Context;
using Hyip_Payments.Models.Reports;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Query.ReportQuery.User
{
    public class GetAuditTrailReportQuery : IRequest<AuditTrailReportModel>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ActionType { get; set; }
        public string? EntityType { get; set; }
        public string? PerformedBy { get; set; }
    }

    public class GetAuditTrailReportQueryHandler : IRequestHandler<GetAuditTrailReportQuery, AuditTrailReportModel>
    {
        private readonly PaymentsDbContext _context;

        public GetAuditTrailReportQueryHandler(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task<AuditTrailReportModel> Handle(GetAuditTrailReportQuery request, CancellationToken cancellationToken)
        {
            // Query REAL audit logs from database
            var query = _context.AuditLogs
                .Where(a => a.Timestamp >= request.StartDate && a.Timestamp <= request.EndDate)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(request.ActionType))
            {
                query = query.Where(a => a.ActionType == request.ActionType);
            }

            if (!string.IsNullOrEmpty(request.EntityType))
            {
                query = query.Where(a => a.EntityType == request.EntityType);
            }

            if (!string.IsNullOrEmpty(request.PerformedBy))
            {
                query = query.Where(a => a.UserName != null && a.UserName.Contains(request.PerformedBy));
            }

            // Get audit logs ordered by timestamp descending (most recent first)
            var auditLogs = await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync(cancellationToken);

            // Convert AuditLogModel to AuditEventDto
            var auditEvents = auditLogs.Select((log, index) => new AuditEventDto
            {
                EventId = index + 1,
                Timestamp = log.Timestamp,
                ActionType = log.ActionType,
                EntityType = log.EntityType,
                EntityId = log.EntityId ?? "N/A",
                PerformedBy = log.UserName ?? log.UserEmail ?? "System",
                UserRole = log.UserRole ?? "User",
                Description = log.Description ?? $"{log.ActionType} {log.EntityType}",
                NewValue = ExtractValue(log.AfterValue),
                OldValue = ExtractValue(log.BeforeValue),
                IpAddress = log.IpAddress ?? "N/A",
                Severity = log.Severity,
                IsSuccessful = log.IsSuccessful
            }).ToList();

            // Create summary from real data
            var summary = new AuditTrailSummaryDto
            {
                CreateActions = auditEvents.Count(e => e.ActionType == "Create"),
                UpdateActions = auditEvents.Count(e => e.ActionType == "Update"),
                DeleteActions = auditEvents.Count(e => e.ActionType == "Delete"),
                ViewActions = auditEvents.Count(e => e.ActionType == "View"),
                LoginActions = auditEvents.Count(e => e.ActionType == "Login"),
                SecurityEvents = auditEvents.Count(e => e.ActionType == "Security"),

                ActionsByType = auditEvents
                    .GroupBy(e => e.ActionType)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ActionsByEntity = auditEvents
                    .GroupBy(e => e.EntityType)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ActionsByUser = auditEvents
                    .GroupBy(e => e.PerformedBy)
                    .ToDictionary(g => g.Key, g => g.Count()),

                ActionsBySeverity = auditEvents
                    .GroupBy(e => e.Severity)
                    .ToDictionary(g => g.Key, g => g.Count()),

                CriticalEvents = auditEvents.Count(e => e.Severity == "Critical"),
                WarningEvents = auditEvents.Count(e => e.Severity == "Warning"),
                InfoEvents = auditEvents.Count(e => e.Severity == "Info")
            };

            // Get most active user
            var mostActiveUser = summary.ActionsByUser.OrderByDescending(u => u.Value).FirstOrDefault();
            summary.MostActiveUser = !string.IsNullOrEmpty(mostActiveUser.Key) ? mostActiveUser.Key : "N/A";
            summary.MostActiveUserActions = mostActiveUser.Value;

            // Get most audited entity
            var mostAuditedEntity = summary.ActionsByEntity.OrderByDescending(e => e.Value).FirstOrDefault();
            summary.MostAuditedEntity = !string.IsNullOrEmpty(mostAuditedEntity.Key) ? mostAuditedEntity.Key : "N/A";
            summary.MostAuditedEntityCount = mostAuditedEntity.Value;

            return new AuditTrailReportModel
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalEvents = auditEvents.Count,
                AuditEvents = auditEvents,
                Summary = summary
            };
        }

        /// <summary>
        /// Extract a readable value from JSON or return as-is
        /// </summary>
        private string? ExtractValue(string? jsonValue)
        {
            if (string.IsNullOrEmpty(jsonValue))
                return null;

            // If it's JSON, try to make it more readable
            if (jsonValue.StartsWith("{") || jsonValue.StartsWith("["))
            {
                try
                {
                    // Truncate long JSON for display
                    if (jsonValue.Length > 200)
                    {
                        return jsonValue.Substring(0, 200) + "...";
                    }
                }
                catch
                {
                    // Ignore JSON parsing errors
                }
            }

            return jsonValue;
        }
    }
}
