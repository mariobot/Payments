using Hyip_Payments.Models.Reports;
using Hyip_Payments.Query.ReportQuery.Customer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerReportController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get customer statement for a specific period
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    /// <param name="startDate">Start date of period</param>
    /// <param name="endDate">End date of period</param>
    /// <param name="includeDraft">Include draft invoices (default: false)</param>
    /// <param name="includeCancelled">Include cancelled invoices (default: false)</param>
    /// <returns>Customer statement report</returns>
    [HttpGet("customer-statement")]
    public async Task<ActionResult<CustomerStatementReportModel>> GetCustomerStatement(
        [FromQuery] int customerId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] bool includeDraft = false,
        [FromQuery] bool includeCancelled = false)
    {
        try
        {
            var query = new GetCustomerStatementQuery
            {
                CustomerId = customerId,
                StartDate = startDate,
                EndDate = endDate,
                IncludeDraft = includeDraft,
                IncludeCancelled = includeCancelled
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating customer statement", error = ex.Message });
        }
    }

    /// <summary>
    /// Get customer statement with POST (for complex requests)
    /// </summary>
    [HttpPost("customer-statement")]
    public async Task<ActionResult<CustomerStatementReportModel>> PostCustomerStatement(
        [FromBody] CustomerStatementRequest request)
    {
        try
        {
            var query = new GetCustomerStatementQuery
            {
                CustomerId = request.CustomerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IncludeDraft = request.IncludeDraft,
                IncludeCancelled = request.IncludeCancelled
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating customer statement", error = ex.Message });
        }
    }
}
