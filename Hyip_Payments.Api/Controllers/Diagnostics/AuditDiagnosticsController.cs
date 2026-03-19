using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Api.Controllers.Diagnostics
{
    /// <summary>
    /// Diagnostic controller to test audit logging and seed sample data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuditDiagnosticsController : ControllerBase
    {
        private readonly PaymentsDbContext _context;

        public AuditDiagnosticsController(PaymentsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Check if AuditLogs table exists and has data
        /// GET: api/AuditDiagnostics/status
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<object>> GetStatus()
        {
            try
            {
                var count = await _context.AuditLogs.CountAsync();
                var latestLog = await _context.AuditLogs
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    TableExists = true,
                    TotalRecords = count,
                    HasData = count > 0,
                    LatestEntry = latestLog != null ? new
                    {
                        latestLog.Id,
                        latestLog.Timestamp,
                        latestLog.ActionType,
                        latestLog.EntityType,
                        latestLog.UserName
                    } : null,
                    Message = count > 0 
                        ? $"AuditLogs table has {count} records" 
                        : "AuditLogs table exists but is empty"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    TableExists = false,
                    Error = ex.Message,
                    Message = "AuditLogs table may not exist. Run the migration: Add-Migration AddAuditLogTable"
                });
            }
        }

        /// <summary>
        /// Seed sample audit log data for testing
        /// POST: api/AuditDiagnostics/seed
        /// </summary>
        [HttpPost("seed")]
        public async Task<ActionResult<object>> SeedData([FromQuery] int count = 50)
        {
            try
            {
                var random = new Random();
                var actionTypes = new[] { "Create", "Update", "Delete", "View", "Login", "Security" };
                var entityTypes = new[] { "Invoice", "Payment", "Customer", "Product", "User", "Order" };
                var severities = new[] { "Info", "Warning", "Critical" };
                var userNames = new[] { "admin", "john.doe", "jane.smith", "bob.johnson", "System" };

                var logs = new List<AuditLogModel>();

                for (int i = 0; i < count; i++)
                {
                    var actionType = actionTypes[random.Next(actionTypes.Length)];
                    var entityType = entityTypes[random.Next(entityTypes.Length)];
                    var userName = userNames[random.Next(userNames.Length)];
                    var severity = actionType == "Security" ? "Warning" : 
                                   actionType == "Delete" ? "Warning" : "Info";

                    var log = new AuditLogModel
                    {
                        Timestamp = DateTime.UtcNow.AddHours(-random.Next(1, 168)), // Last 7 days
                        UserId = $"user-{random.Next(1, 10)}",
                        UserName = userName,
                        UserEmail = $"{userName.Replace(".", "")}@example.com",
                        ActionType = actionType,
                        EntityType = entityType,
                        EntityId = random.Next(1, 1000).ToString(),
                        IpAddress = $"192.168.1.{random.Next(1, 255)}",
                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/120.0.0.0",
                        BeforeValue = actionType == "Update" || actionType == "Delete" 
                            ? $"{{\"id\": {random.Next(1, 100)}, \"status\": \"old\"}}" 
                            : null,
                        AfterValue = actionType == "Create" || actionType == "Update" 
                            ? $"{{\"id\": {random.Next(1, 100)}, \"status\": \"new\"}}" 
                            : null,
                        Severity = severity,
                        IsSuccessful = random.Next(10) > 1, // 90% success rate
                        Description = $"{actionType} {entityType} #{random.Next(1, 1000)}",
                        HttpMethod = actionType == "Create" ? "POST" : 
                                   actionType == "Update" ? "PUT" :
                                   actionType == "Delete" ? "DELETE" : "GET",
                        RequestPath = $"/api/{entityType.ToLower()}/{random.Next(1, 100)}",
                        UserRole = userName == "admin" ? "Admin" : "User",
                        DurationMs = random.Next(10, 500),
                        SessionId = Guid.NewGuid().ToString().Substring(0, 16),
                        CorrelationId = Guid.NewGuid().ToString()
                    };

                    logs.Add(log);
                }

                await _context.AuditLogs.AddRangeAsync(logs);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    RecordsCreated = count,
                    Message = $"Successfully seeded {count} audit log records",
                    DateRange = new
                    {
                        From = logs.Min(l => l.Timestamp),
                        To = logs.Max(l => l.Timestamp)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Clear all audit log data
        /// DELETE: api/AuditDiagnostics/clear
        /// </summary>
        [HttpDelete("clear")]
        public async Task<ActionResult<object>> ClearData()
        {
            try
            {
                var count = await _context.AuditLogs.CountAsync();
                _context.AuditLogs.RemoveRange(_context.AuditLogs);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    RecordsDeleted = count,
                    Message = $"Successfully deleted {count} audit log records"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get recent audit logs for debugging
        /// GET: api/AuditDiagnostics/recent?count=10
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<object>> GetRecentLogs([FromQuery] int count = 10)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .Select(a => new
                    {
                        a.Id,
                        a.Timestamp,
                        a.ActionType,
                        a.EntityType,
                        a.EntityId,
                        a.UserName,
                        a.IpAddress,
                        a.Severity,
                        a.IsSuccessful,
                        a.Description
                    })
                    .ToListAsync();

                return Ok(new
                {
                    TotalCount = await _context.AuditLogs.CountAsync(),
                    RecentLogs = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message
                });
            }
        }
    }
}
