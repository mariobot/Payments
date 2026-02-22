using Hyip_Payments.Models.Reports;
using Hyip_Payments.Query.ReportQuery.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/UserReport/activity?startDate=2024-01-01&endDate=2024-12-31&role=Admin
        /// <summary>
        /// Get user activity report for a specific date range
        /// </summary>
        [HttpGet("activity")]
        public async Task<ActionResult<UserActivityReportModel>> GetActivity(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate,
            [FromQuery] string? role = null)
        {
            var query = new GetUserActivityReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                Role = role
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/UserReport/audit-trail?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get audit trail report with optional filters
        /// </summary>
        [HttpGet("audit-trail")]
        public async Task<ActionResult<AuditTrailReportModel>> GetAuditTrail(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate,
            [FromQuery] string? actionType = null,
            [FromQuery] string? entityType = null,
            [FromQuery] string? performedBy = null)
        {
            var query = new GetAuditTrailReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                ActionType = actionType,
                EntityType = entityType,
                PerformedBy = performedBy
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/UserReport/active-users?reportDate=2024-12-31
        /// <summary>
        /// Get active users report showing currently active users and engagement metrics
        /// </summary>
        [HttpGet("active-users")]
        public async Task<ActionResult<ActiveUsersReportModel>> GetActiveUsers(
            [FromQuery] DateTime? reportDate = null)
        {
            var query = new GetActiveUsersReportQuery 
            { 
                ReportDate = reportDate ?? DateTime.UtcNow
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/UserReport/user-roles?reportDate=2024-12-31
        /// <summary>
        /// Get user roles report showing distribution and activity by role
        /// </summary>
        [HttpGet("user-roles")]
        public async Task<ActionResult<UserRolesReportModel>> GetUserRoles(
            [FromQuery] DateTime? reportDate = null)
        {
            var query = new GetUserRolesReportQuery 
            { 
                ReportDate = reportDate ?? DateTime.UtcNow
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
