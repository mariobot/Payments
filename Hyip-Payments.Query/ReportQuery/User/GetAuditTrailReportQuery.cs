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
            // Generate audit trail events ONLY from actual database data
            // Note: This is a best-effort approach since you don't have a dedicated AuditLog table
            var auditEvents = new List<AuditEventDto>();
            int eventId = 1;

            // Invoice creation events - REAL DATA
            var invoices = await _context.Invoices
                .Where(i => i.InvoiceDate >= request.StartDate && i.InvoiceDate <= request.EndDate)
                .OrderByDescending(i => i.InvoiceDate)
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
                    Description = $"Invoice {invoice.InvoiceNumber} created for ${invoice.TotalAmount:F2}",
                    NewValue = $"Amount: ${invoice.TotalAmount:F2}, Status: {invoice.StatusInvoice}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Payment transaction events - REAL DATA
            var payments = await _context.PaymentTransactions
                .Where(p => p.TransactionDate >= request.StartDate && p.TransactionDate <= request.EndDate)
                .OrderByDescending(p => p.TransactionDate)
                .ToListAsync(cancellationToken);

            foreach (var payment in payments)
            {
                var severity = payment.Status == "Failed" ? "Warning" : "Info";
                var actionType = payment.Status == "Cancelled" ? "Delete" : "Create";

                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = payment.TransactionDate,
                    ActionType = actionType,
                    EntityType = "Payment",
                    EntityId = payment.Id.ToString(),
                    PerformedBy = payment.ProcessedByUserId ?? "System",
                    UserRole = "User",
                    Description = $"Payment transaction {(payment.Reference ?? payment.Id.ToString())} - Status: {payment.Status}",
                    NewValue = $"Amount: ${payment.Amount:F2}, Status: {payment.Status}, Method: {payment.PaymentMethodId}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = severity,
                    IsSuccessful = payment.Status == "Completed"
                });
            }

            // Product creation events - REAL DATA (limited to recent products to avoid overload)
            var productStartDate = request.StartDate > DateTime.UtcNow.AddMonths(-3) 
                ? request.StartDate 
                : DateTime.UtcNow.AddMonths(-3); // Limit to last 3 months for performance

            var products = await _context.Products
                .Where(p => p.CreatedAt >= productStartDate && p.CreatedAt <= request.EndDate)
                .OrderByDescending(p => p.CreatedAt)
                .Take(100) // Limit to 100 most recent products
                .ToListAsync(cancellationToken);

            foreach (var product in products)
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = product.CreatedAt,
                    ActionType = "Create",
                    EntityType = "Product",
                    EntityId = product.Id.ToString(),
                    PerformedBy = "System", // Products don't have creator tracking
                    UserRole = "User",
                    Description = $"Product '{product.Name}' created",
                    NewValue = $"Price: ${product.Price:F2}, Active: {product.IsActive}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Customer creation events - REAL DATA
            var customerStartDate = request.StartDate > DateTime.UtcNow.AddMonths(-3) 
                ? request.StartDate 
                : DateTime.UtcNow.AddMonths(-3); // Limit to last 3 months for performance

            var customers = await _context.Customers
                .Where(c => c.CreatedAt >= customerStartDate && c.CreatedAt <= request.EndDate)
                .OrderByDescending(c => c.CreatedAt)
                .Take(100) // Limit to 100 most recent customers
                .ToListAsync(cancellationToken);

            foreach (var customer in customers)
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = customer.CreatedAt,
                    ActionType = "Create",
                    EntityType = "Customer",
                    EntityId = customer.Id.ToString(),
                    PerformedBy = "System", // Customers don't have creator tracking
                    UserRole = "User",
                    Description = $"Customer '{customer.CompanyName}' created",
                    NewValue = $"Customer #: {customer.CustomerNumber}, Email: {customer.Email}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Recurring Invoice events - REAL DATA
            var recurringInvoices = await _context.RecurringInvoices
                .Where(r => r.CreatedAt >= request.StartDate && r.CreatedAt <= request.EndDate)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(cancellationToken);

            foreach (var recurring in recurringInvoices)
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = recurring.CreatedAt,
                    ActionType = "Create",
                    EntityType = "Recurring Invoice",
                    EntityId = recurring.Id.ToString(),
                    PerformedBy = "System",
                    UserRole = "User",
                    Description = $"Recurring invoice template '{recurring.TemplateName}' created",
                    NewValue = $"Frequency: {recurring.Frequency}, Active: {recurring.IsActive}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = "Info",
                    IsSuccessful = true
                });

                // Add event for each generated invoice
                if (recurring.LastGeneratedDate.HasValue && 
                    recurring.LastGeneratedDate >= request.StartDate && 
                    recurring.LastGeneratedDate <= request.EndDate)
                {
                    auditEvents.Add(new AuditEventDto
                    {
                        EventId = eventId++,
                        Timestamp = recurring.LastGeneratedDate.Value,
                        ActionType = "Create",
                        EntityType = "Invoice",
                        EntityId = $"Generated-{recurring.Id}",
                        PerformedBy = "System (Auto)",
                        UserRole = "System",
                        Description = $"Invoice auto-generated from template '{recurring.TemplateName}'",
                        NewValue = $"Template: {recurring.TemplateName}, Count: {recurring.GeneratedInvoiceCount}",
                        IpAddress = "N/A",
                        Severity = "Info",
                        IsSuccessful = true
                    });
                }
            }

            // User creation events - REAL DATA
            var userStartDate = request.StartDate > DateTime.UtcNow.AddMonths(-3) 
                ? request.StartDate 
                : DateTime.UtcNow.AddMonths(-3); // Limit to last 3 months for performance

            var users = await _context.Users
                .Where(u => u.CreatedAt >= userStartDate && u.CreatedAt <= request.EndDate)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                auditEvents.Add(new AuditEventDto
                {
                    EventId = eventId++,
                    Timestamp = user.CreatedAt,
                    ActionType = "Create",
                    EntityType = "User",
                    EntityId = user.Id.ToString(),
                    PerformedBy = "System",
                    UserRole = "Admin",
                    Description = $"User '{user.Username}' created in the system",
                    NewValue = $"Email: {user.Email}, Active: {user.IsActive}",
                    IpAddress = "N/A", // Not tracked - no IP data available
                    Severity = "Info",
                    IsSuccessful = true
                });
            }

            // Apply filters
            if (!string.IsNullOrEmpty(request.ActionType))
            {
                auditEvents = auditEvents
                    .Where(e => e.ActionType.Equals(request.ActionType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(request.EntityType))
            {
                auditEvents = auditEvents
                    .Where(e => e.EntityType.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(request.PerformedBy))
            {
                auditEvents = auditEvents
                    .Where(e => e.PerformedBy.Contains(request.PerformedBy, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sort by timestamp descending (most recent first)
            auditEvents = auditEvents.OrderByDescending(e => e.Timestamp).ToList();

            // Create summary from REAL data only
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
            summary.MostActiveUser = !string.IsNullOrEmpty(mostActiveUser.Key) ? mostActiveUser.Key : "N/A";
            summary.MostActiveUserActions = mostActiveUser.Value;

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
    }
}
