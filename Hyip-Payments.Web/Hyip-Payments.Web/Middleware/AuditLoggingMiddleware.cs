using Hyip_Payments.Services;
using System.Diagnostics;

namespace Hyip_Payments.Web.Middleware
{
    /// <summary>
    /// Middleware to automatically log HTTP requests and responses
    /// </summary>
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLoggingMiddleware> _logger;

        // Paths that should be audited
        private static readonly HashSet<string> AuditPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/invoice",
            "/api/payment",
            "/api/customer",
            "/api/product",
            "/api/country",
            "/api/money",
            "/api/wallet",
            "/api/coin",
            "/api/brand",
            "/api/category",
            "/api/recurringinvoice",
            "/api/paymenttransaction"
        };

        public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            // Check if this is an API endpoint that should be audited
            var path = context.Request.Path.ToString();
            if (!ShouldAudit(path, context.Request.Method))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                // Call the next middleware
                await _next(context);

                stopwatch.Stop();

                // Log the request if it modified data (POST, PUT, DELETE)
                if (IsModifyingRequest(context.Request.Method))
                {
                    await LogRequest(context, auditService, stopwatch.ElapsedMilliseconds);
                }

                // Copy response body back
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log failed requests
                await LogFailedRequest(context, auditService, ex, stopwatch.ElapsedMilliseconds);

                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool ShouldAudit(string path, string method)
        {
            // Only audit data-modifying operations (POST, PUT, DELETE)
            if (!IsModifyingRequest(method))
                return false;

            // Check if path starts with any audit path
            return AuditPaths.Any(auditPath => 
                path.StartsWith(auditPath, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsModifyingRequest(string method)
        {
            return method == "POST" || method == "PUT" || method == "DELETE";
        }

        private async Task LogRequest(HttpContext context, IAuditService auditService, long durationMs)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            var statusCode = context.Response.StatusCode;

            // Determine action type and entity type from path
            var (actionType, entityType, entityId) = ParseRequestInfo(method, path);

            var isSuccessful = statusCode >= 200 && statusCode < 300;
            var severity = isSuccessful ? "Info" : "Warning";

            try
            {
                await auditService.LogActionAsync(
                    actionType: actionType,
                    entityType: entityType,
                    entityId: entityId,
                    description: $"{actionType} {entityType} via HTTP {method}",
                    severity: severity,
                    isSuccessful: isSuccessful
                );
            }
            catch (Exception ex)
            {
                // Log error but don't throw - audit logging should not break the application
                _logger.LogError(ex, "Failed to log audit event for {Method} {Path}", method, path);
            }
        }

        private async Task LogFailedRequest(HttpContext context, IAuditService auditService, Exception exception, long durationMs)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();

            var (actionType, entityType, entityId) = ParseRequestInfo(method, path);

            try
            {
                await auditService.LogFailureAsync(
                    actionType: actionType,
                    entityType: entityType,
                    entityId: entityId,
                    errorMessage: exception.Message
                );
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                _logger.LogError(ex, "Failed to log failed audit event for {Method} {Path}", method, path);
            }
        }

        private (string actionType, string entityType, string? entityId) ParseRequestInfo(string method, string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Determine action type from HTTP method
            var actionType = method switch
            {
                "POST" => "Create",
                "PUT" => "Update",
                "DELETE" => "Delete",
                _ => "Unknown"
            };

            // Extract entity type from path (e.g., /api/invoice -> Invoice)
            var entityType = segments.Length >= 2 ? segments[1] : "Unknown";
            
            // Capitalize first letter
            if (!string.IsNullOrEmpty(entityType))
            {
                entityType = char.ToUpper(entityType[0]) + entityType.Substring(1);
            }

            // Extract entity ID if present (e.g., /api/invoice/123 -> "123")
            var entityId = segments.Length >= 3 ? segments[2] : null;

            return (actionType, entityType, entityId);
        }
    }

    /// <summary>
    /// Extension method to register audit logging middleware
    /// </summary>
    public static class AuditLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditLoggingMiddleware>();
        }
    }
}
