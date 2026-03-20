using Hyip_Payments.Context;
using Hyip_Payments.Models;
using System.Text.Json;

namespace Hyip_Payments.Services
{
    /// <summary>
    /// Implementation of audit logging service with data sanitization
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

            // Sanitize sensitive data before saving
            auditLog.BeforeValue = AuditDataSanitizer.SanitizeJson(auditLog.BeforeValue);
            auditLog.AfterValue = AuditDataSanitizer.SanitizeJson(auditLog.AfterValue);
            auditLog.AdditionalData = AuditDataSanitizer.SanitizeJson(auditLog.AdditionalData);

            // Sanitize description if it contains sensitive keywords
            if (AuditDataSanitizer.ContainsSensitiveData(auditLog.Description))
            {
                auditLog.Description = "[REDACTED - Contains sensitive information]";
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
                // Sanitize objects before serialization
                BeforeValue = AuditDataSanitizer.SanitizeObject(beforeValue),
                AfterValue = AuditDataSanitizer.SanitizeObject(afterValue)
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
                afterValue: createdObject // Will be sanitized in LogActionAsync
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
                beforeValue: beforeObject, // Will be sanitized in LogActionAsync
                afterValue: afterObject    // Will be sanitized in LogActionAsync
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
                beforeValue: deletedObject, // Will be sanitized in LogActionAsync
                afterValue: null
            );
        }

        public async Task LogLoginAsync(string userId, string userName, bool isSuccessful, string? ipAddress = null)
        {
            // NEVER log password - login attempts should not include sensitive data
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
                IpAddress = ipAddress,
                // NO password data logged - only the fact that login occurred
                BeforeValue = null,
                AfterValue = null
            };

            await LogAsync(auditLog);
        }

        public async Task LogSecurityEventAsync(string description, string severity = "Warning", string? userId = null)
        {
            // Sanitize description to ensure no sensitive data leaks
            var sanitizedDescription = AuditDataSanitizer.ContainsSensitiveData(description)
                ? "[REDACTED - Contains sensitive keywords]"
                : description;

            var auditLog = new AuditLogModel
            {
                ActionType = "Security",
                EntityType = "System",
                Description = sanitizedDescription,
                Severity = severity,
                UserId = userId,
                IsSuccessful = false
            };

            await LogAsync(auditLog);
        }

        public async Task LogFailureAsync(string actionType, string entityType, string? entityId, string errorMessage, string? userId = null)
        {
            // Sanitize error message to avoid leaking sensitive data in exceptions
            var sanitizedError = AuditDataSanitizer.ContainsSensitiveData(errorMessage)
                ? "[REDACTED - Error contains sensitive information]"
                : errorMessage;

            var auditLog = new AuditLogModel
            {
                ActionType = actionType,
                EntityType = entityType,
                EntityId = entityId,
                UserId = userId,
                Description = $"Failed to {actionType.ToLower()} {entityType}: {sanitizedError}",
                Severity = "Critical",
                IsSuccessful = false,
                AdditionalData = JsonSerializer.Serialize(new { ErrorMessage = sanitizedError })
            };

            await LogAsync(auditLog);
        }
    }
}
