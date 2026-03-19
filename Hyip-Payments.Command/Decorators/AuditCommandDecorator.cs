using Hyip_Payments.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Hyip_Payments.Command.Decorators
{
    /// <summary>
    /// Decorator to automatically audit MediatR commands
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class AuditCommandDecorator<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<AuditCommandDecorator<TRequest, TResponse>> _logger;

        public AuditCommandDecorator(
            IAuditService auditService,
            ILogger<AuditCommandDecorator<TRequest, TResponse>> logger)
        {
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var commandName = requestType.Name;

            // Only audit commands (not queries)
            if (!commandName.EndsWith("Command"))
            {
                return await next();
            }

            var startTime = DateTime.UtcNow;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Execute the command
                var response = await next();

                stopwatch.Stop();

                // Log successful command execution
                await LogCommandExecution(request, response, commandName, stopwatch.ElapsedMilliseconds, true, null);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                // Log failed command execution
                await LogCommandExecution(request, default(TResponse), commandName, stopwatch.ElapsedMilliseconds, false, ex);

                throw;
            }
        }

        private async Task LogCommandExecution(
            TRequest request,
            TResponse? response,
            string commandName,
            long durationMs,
            bool isSuccessful,
            Exception? exception)
        {
            try
            {
                var (actionType, entityType, entityId) = ParseCommandInfo(commandName, request);

                var auditLog = new Models.AuditLogModel
                {
                    ActionType = actionType,
                    EntityType = entityType,
                    EntityId = entityId,
                    Description = $"{commandName} executed",
                    Severity = isSuccessful ? "Info" : "Critical",
                    IsSuccessful = isSuccessful,
                    DurationMs = durationMs,
                    BeforeValue = null, // Commands don't typically have before values
                    AfterValue = response != null ? JsonSerializer.Serialize(response) : null,
                    AdditionalData = exception != null 
                        ? JsonSerializer.Serialize(new { 
                            ExceptionType = exception.GetType().Name,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace
                        })
                        : null
                };

                await _auditService.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                // Don't throw - audit logging should not break the application
                _logger.LogError(ex, "Failed to log audit for command {CommandName}", commandName);
            }
        }

        private (string actionType, string entityType, string? entityId) ParseCommandInfo(string commandName, TRequest request)
        {
            // Extract action type from command name
            // Examples: AddInvoiceCommand -> Create, UpdateInvoiceCommand -> Update
            var actionType = "Unknown";
            var entityType = "Unknown";
            string? entityId = null;

            if (commandName.StartsWith("Add") || commandName.StartsWith("Create"))
            {
                actionType = "Create";
                entityType = commandName.Replace("Add", "").Replace("Create", "").Replace("Command", "");
            }
            else if (commandName.StartsWith("Update") || commandName.StartsWith("Edit"))
            {
                actionType = "Update";
                entityType = commandName.Replace("Update", "").Replace("Edit", "").Replace("Command", "");
                
                // Try to get ID from request using reflection
                entityId = GetEntityId(request);
            }
            else if (commandName.StartsWith("Delete") || commandName.StartsWith("Remove"))
            {
                actionType = "Delete";
                entityType = commandName.Replace("Delete", "").Replace("Remove", "").Replace("Command", "");
                
                // Try to get ID from request using reflection
                entityId = GetEntityId(request);
            }

            return (actionType, entityType, entityId);
        }

        private string? GetEntityId(TRequest request)
        {
            try
            {
                // Try to get Id property using reflection
                var idProperty = request.GetType().GetProperty("Id");
                if (idProperty != null)
                {
                    var id = idProperty.GetValue(request);
                    return id?.ToString();
                }

                // Try alternative property names
                foreach (var propName in new[] { "EntityId", "InvoiceId", "PaymentId", "CustomerId", "ProductId" })
                {
                    var prop = request.GetType().GetProperty(propName);
                    if (prop != null)
                    {
                        var id = prop.GetValue(request);
                        return id?.ToString();
                    }
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            return null;
        }
    }
}
