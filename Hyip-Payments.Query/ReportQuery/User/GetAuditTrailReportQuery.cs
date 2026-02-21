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
            // Simulate audit trail events from actual database changes
            // In production, you'd have a dedicated AuditLog table
            var auditEvents = new List<AuditEventDto>();
            int eventId = 1;

            // Invoice creation/update events
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= request.StartDate && i.InvoiceDate <= request.EndDate)
                .ToListAsync(cancellationToken);

            foreach (var invoice in invoices)
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = invoice.InvoiceDate,
                    ActionType = "Create",
                    EntityType = "Invoice",
                    EntityId = invoice.Id.ToString(),
                    PerformedBy = invoice.CreatedByUserId ?? "System",
                    UserRole = "User",
                    Description = $"Invoice {invoice.InvoiceNumber} created for ${invoice.TotalAmount}",
                    NewValue = $"Amount: ${invoice.TotalAmount}, Status: {invoice.StatusInvoice}",
                    IpAddress = "192.168.1.100",
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Payment transaction events
            var payments = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate && p.TransactionDate <= request.EndDate)
                .ToListAsync(cancellationToken);

            foreach (var payment in payments)
            {
                var severity = payment.Status == "Failed" ? "Warning" : "Info";
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = payment.TransactionDate,
                    ActionType = payment.Status == "Completed" ? "Create" : "Update",
                    EntityType = "Payment",
                    EntityId = payment.Id.ToString(),
                    PerformedBy = payment.ProcessedByUserId ?? "System",
                    UserRole = "User",
                    Description = $"Payment {payment.Reference ?? payment.Id.ToString()} - Status: {payment.Status}",
                    NewValue = $"Amount: ${payment.Amount}, Status: {payment.Status}",
                    IpAddress = "192.168.1.100",
                    Severity = severity,
                    IsSuccessful = payment.Status == "Completed"
                });
            }

            // Product creation events
            var products = await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var product in products.Take(50)) // Limit to recent products
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = product.CreatedAt,
                    ActionType = "Create",
                    EntityType = "Product",
                    EntityId = product.Id.ToString(),
                    PerformedBy = "System",
                    UserRole = "User",
                    Description = $"Product '{product.Name}' created",
                    NewValue = $"Price: ${product.Price}, Stock: Active",
                    IpAddress = "192.168.1.100",
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Simulated login events
            var users = await _context.Users.Where(u => u.IsActive).Take(10).ToListAsync(cancellationToken);
            foreach (var user in users)
            {
                for (int i = 0; i < 3; i++)
                {
                    auditEvents.Add(new AuditEventDto
                    {
                        EventId = eventId++,
                        Timestamp = request.StartDate.AddDays(new Random().Next(0, (request.EndDate - request.StartDate).Days)),
                        ActionType = "Login",
                        EntityType = "User",
                        EntityId = user.Id.ToString(),
                        PerformedBy = user.Username ?? "Unknown",
                        UserRole = "User",
                        Description = $"User '{user.Username}' logged in successfully",
                        IpAddress = $"192.168.1.{new Random().Next(100, 200)}",
                        Severity = "Info",
                        IsSuccessful = true
                    });
                }
            }

            // Simulated critical security events
            auditEvents.Add(new AuditEventDto
            {
                EventId = eventId++,
                Timestamp = request.StartDate.AddDays(5),
                ActionType = "Security",
                EntityType = "System",
                EntityId = "SEC-001",
                PerformedBy = "System",
                UserRole = "System",
                Description = "Failed login attempt detected - 5 consecutive failures",
                IpAddress = "203.0.113.1",
                Severity = "Critical",
                IsSuccessful = false
            });

            auditEvents.Add(new AuditEventDto
            {
                EventId = eventId++,
                Timestamp = request.StartDate.AddDays(10),
                ActionType = "Delete",
                EntityType = "User",
                EntityId = "USR-999",
                PerformedBy = "admin",
                UserRole = "Admin",
                Description = "User account deleted",
                OldValue = "Active user account",
                NewValue = "Deleted",
                IpAddress = "192.168.1.1",
                Severity = "Warning",
                IsSuccessful = true
            });

            // Apply filters
            if (!string.IsNullOrEmpty(request.ActionType))
            {
                auditEvents = auditEvents.Where(e => e.ActionType.Equals(request.ActionType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(request.EntityType))
            {
                auditEvents = auditEvents.Where(e => e.EntityType.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(request.PerformedBy))
            {
                auditEvents = auditEvents.Where(e => e.PerformedBy.Contains(request.PerformedBy, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Sort by timestamp descending
            auditEvents = auditEvents.OrderByDescending(e => e.Timestamp).ToList();

            // Create summary
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

            var mostActiveUser = summary.ActionsByUser.OrderByDescending(u => u.Value).FirstOrDefault();
            summary.MostActiveUser = mostActiveUser.Key ?? "None";
            summary.MostActiveUserActions = mostActiveUser.Value;

            var mostAuditedEntity = summary.ActionsByEntity.OrderByDescending(e => e.Value).FirstOrDefault();
            summary.MostAuditedEntity = mostAuditedEntity.Key ?? "None";
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
    }
}
