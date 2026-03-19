using Hyip_Payments.Models;

namespace Hyip_Payments.Services
{
    /// <summary>
    /// Service for logging audit trail events
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log a complete audit event
        /// </summary>
        Task LogAsync(AuditLogModel auditLog);

        /// <summary>
        /// Log an action with simplified parameters
        /// </summary>
        Task LogActionAsync(
            string actionType,
            string entityType,
            string? entityId = null,
            string? description = null,
            string? userId = null,
            string? userName = null,
            string severity = "Info",
            bool isSuccessful = true,
            object? beforeValue = null,
            object? afterValue = null);

        /// <summary>
        /// Log a create action
        /// </summary>
        Task LogCreateAsync(string entityType, string entityId, object createdObject, string? userId = null, string? userName = null);

        /// <summary>
        /// Log an update action
        /// </summary>
        Task LogUpdateAsync(string entityType, string entityId, object beforeObject, object afterObject, string? userId = null, string? userName = null);

        /// <summary>
        /// Log a delete action
        /// </summary>
        Task LogDeleteAsync(string entityType, string entityId, object deletedObject, string? userId = null, string? userName = null);

        /// <summary>
        /// Log a login event
        /// </summary>
        Task LogLoginAsync(string userId, string userName, bool isSuccessful, string? ipAddress = null);

        /// <summary>
        /// Log a security event
        /// </summary>
        Task LogSecurityEventAsync(string description, string severity = "Warning", string? userId = null);

        /// <summary>
        /// Log a failed operation
        /// </summary>
        Task LogFailureAsync(string actionType, string entityType, string? entityId, string errorMessage, string? userId = null);
    }
}
