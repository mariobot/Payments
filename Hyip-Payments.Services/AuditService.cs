using Hyip_Payments.Context;
using Hyip_Payments.Models;
using System.Text.Json;

namespace Hyip_Payments.Services
{
    /// <summary>
    /// Implementation of audit logging service
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly PaymentsDbContext _context;

        public AuditService(PaymentsDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(AuditLogModel auditLog)
        {
            // Ensure timestamp is set
            if (auditLog.Timestamp == default)
            {
                auditLog.Timestamp = DateTime.UtcNow;
            }

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task LogActionAsync(
            string actionType,
            string entityType,
            string? entityId = null,
            string? description = null,
            string? userId = null,
            string? userName = null,
            string severity = "Info",
            bool isSuccessful = true,
            object? beforeValue = null,
            object? afterValue = null)
        {
            var auditLog = new AuditLogModel
            {
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                UserId = userId,
                UserName = userName,
                Severity = severity,
                IsSuccessful = isSuccessful,
                BeforeValue = beforeValue != null ? JsonSerializer.Serialize(beforeValue) : null,
                AfterValue = afterValue != null ? JsonSerializer.Serialize(afterValue) : null
            };

            await LogAsync(auditLog);
        }

        public async Task LogCreateAsync(string entityType, string entityId, object createdObject, string? userId = null, string? userName = null)
        {
            await LogActionAsync(
                actionType: "Create",
                entityType: entityType,
                entityId: entityId,
                description: $"{entityType} created",
                userId: userId,
                userName: userName,
                severity: "Info",
                isSuccessful: true,
                beforeValue: null,
                afterValue: createdObject
            );
        }

        public async Task LogUpdateAsync(string entityType, string entityId, object beforeObject, object afterObject, string? userId = null, string? userName = null)
        {
            await LogActionAsync(
                actionType: "Update",
                entityType: entityType,
                entityId: entityId,
                description: $"{entityType} updated",
                userId: userId,
                userName: userName,
                severity: "Info",
                isSuccessful: true,
                beforeValue: beforeObject,
                afterValue: afterObject
            );
        }

        public async Task LogDeleteAsync(string entityType, string entityId, object deletedObject, string? userId = null, string? userName = null)
        {
            await LogActionAsync(
                actionType: "Delete",
                entityType: entityType,
                entityId: entityId,
                description: $"{entityType} deleted",
                userId: userId,
                userName: userName,
                severity: "Warning",
                isSuccessful: true,
                beforeValue: deletedObject,
                afterValue: null
            );
        }

        public async Task LogLoginAsync(string userId, string userName, bool isSuccessful, string? ipAddress = null)
        {
            var auditLog = new AuditLogModel
            {
                ActionType = "Login",
                EntityType = "User",
                EntityId = userId,
                UserId = userId,
                UserName = userName,
                Description = isSuccessful ? "User logged in successfully" : "Failed login attempt",
                Severity = isSuccessful ? "Info" : "Warning",
                IsSuccessful = isSuccessful,
                IpAddress = ipAddress
            };

            await LogAsync(auditLog);
        }

        public async Task LogSecurityEventAsync(string description, string severity = "Warning", string? userId = null)
        {
            var auditLog = new AuditLogModel
            {
                ActionType = "Security",
                EntityType = "System",
                Description = description,
                Severity = severity,
                UserId = userId,
                IsSuccessful = false
            };

            await LogAsync(auditLog);
        }

        public async Task LogFailureAsync(string actionType, string entityType, string? entityId, string errorMessage, string? userId = null)
        {
            var auditLog = new AuditLogModel
            {
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Description = $"Failed to {actionType.ToLower()} {entityType}: {errorMessage}",
                Severity = "Critical",
                IsSuccessful = false,
                AdditionalData = JsonSerializer.Serialize(new { ErrorMessage = errorMessage })
            };

            await LogAsync(auditLog);
        }
    }
}
