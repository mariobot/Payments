using Hyip_Payments.Models.Reports;
using Hyip_Payments.Query.ReportQuery.Financial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FinancialReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinancialReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/FinancialReport/revenue?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get revenue report for a specific date range
        /// </summary>
        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueReportModel>> GetRevenue(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var query = new GetRevenueReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate 
            };
            
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
