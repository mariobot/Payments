using Hyip_Payments.Context;
using Hyip_Payments.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Api.Controllers.Diagnostics
{
    /// <summary>
    /// Diagnostic controller to test audit logging
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
                        : "AuditLogs table exists but is empty. Create invoices, payments, or other entities to populate audit logs."
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
        /// Clear all audit log data
        /// DELETE: api/AuditDiagnostics/clear
        /// </summary>
        [HttpDelete("clear")]
        [Authorize(Roles = "Admin")] // Restrict to admins only
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
