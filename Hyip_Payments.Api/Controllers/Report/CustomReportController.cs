using System.Security.Claims;
using Hyip_Payments.Command.CustomReportCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CustomReportQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CustomReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/CustomReport
        [HttpGet]
        public async Task<IActionResult> GetSavedReports()
        {
            var userId = GetCurrentUserId();
            var reports = await _mediator.Send(new GetSavedReportsQuery(userId));
            return Ok(reports);
        }

        // GET: api/CustomReport/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var report = await _mediator.Send(new GetCustomReportByIdQuery(id));
            if (report == null)
                return NotFound();

            // Check if user has access to this report
            var userId = GetCurrentUserId();
            if (report.CreatedByUserId != userId && !report.IsPublic)
                return Forbid();

            return Ok(report);
        }

        // POST: api/CustomReport
        [HttpPost]
        public async Task<IActionResult> SaveReport([FromBody] CustomReportModel report)
        {
            var userId = GetCurrentUserId();
            report.CreatedByUserId = userId;
            report.CreatedDate = DateTime.UtcNow;

            var result = await _mediator.Send(new SaveCustomReportCommand(report));
            return CreatedAtAction(nameof(GetReport), new { id = result.Id }, result);
        }

        // PUT: api/CustomReport/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] CustomReportModel report)
        {
            if (id != report.Id)
                return BadRequest("Report ID mismatch");

            // Check ownership
            var existing = await _mediator.Send(new GetCustomReportByIdQuery(id));
            if (existing == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (existing.CreatedByUserId != userId)
                return Forbid("You can only edit your own reports");

            var result = await _mediator.Send(new UpdateCustomReportCommand(report));
            return Ok(result);
        }

        // DELETE: api/CustomReport/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            // Check ownership
            var existing = await _mediator.Send(new GetCustomReportByIdQuery(id));
            if (existing == null)
                return NotFound();

            var userId = GetCurrentUserId();
            if (existing.CreatedByUserId != userId)
                return Forbid("You can only delete your own reports");

            var result = await _mediator.Send(new DeleteCustomReportCommand(id));
            if (!result)
                return NotFound();

            return NoContent();
        }

        // PUT: api/CustomReport/5/run
        [HttpPut("{id}/run")]
        public async Task<IActionResult> IncrementRunCount(int id)
        {
            var result = await _mediator.Send(new IncrementReportRunCountCommand(id));
            if (!result)
                return NotFound();

            return NoContent();
        }

        // POST: api/CustomReport/execute
        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteReport([FromBody] ExecuteReportRequest request)
        {
            var query = new ExecuteCustomReportQuery(
                request.DataSource,
                request.SelectedColumns,
                request.StartDate,
                request.EndDate,
                request.IncludeCompleted,
                request.IncludePending,
                request.IncludeFailed,
                request.IncludeCancelled,
                request.SortBy
            );

            var results = await _mediator.Send(query);
            return Ok(results);
        }

        // Helper method to get current user ID
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value 
                ?? User.FindFirst("userId")?.Value 
                ?? string.Empty;
        }
    }

    // Request model for executing reports
    public class ExecuteReportRequest
    {
        public string DataSource { get; set; } = string.Empty;
        public List<string> SelectedColumns { get; set; } = new();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeCompleted { get; set; }
        public bool IncludePending { get; set; }
        public bool IncludeFailed { get; set; }
        public bool IncludeCancelled { get; set; }
        public string SortBy { get; set; } = string.Empty;
    }
}
