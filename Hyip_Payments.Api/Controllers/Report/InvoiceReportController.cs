using Hyip_Payments.Models.Reports;
using Hyip_Payments.Query.ReportQuery.Invoice;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/InvoiceReport/summary?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get invoice summary report for a specific date range
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<InvoiceSummaryReportModel>> GetSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var query = new GetInvoiceSummaryReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate 
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/InvoiceReport/aging?reportDate=2024-01-15
        /// <summary>
        /// Get invoice aging report showing unpaid invoices by aging buckets
        /// </summary>
        [HttpGet("aging")]
        public async Task<ActionResult<InvoiceAgingReportModel>> GetAging(
            [FromQuery] DateTime? reportDate = null)
        {
            var query = new GetInvoiceAgingReportQuery 
            { 
                ReportDate = reportDate ?? DateTime.Today
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/InvoiceReport/drafts?reportDate=2024-01-15
        /// <summary>
        /// Get draft invoices report showing all invoices in draft status
        /// </summary>
        [HttpGet("drafts")]
        public async Task<ActionResult<DraftInvoicesReportModel>> GetDrafts(
            [FromQuery] DateTime? reportDate = null)
        {
            var query = new GetDraftInvoicesReportQuery 
            { 
                ReportDate = reportDate ?? DateTime.Today
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/InvoiceReport/paid?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get paid invoices report showing all invoices that have been paid
        /// </summary>
        [HttpGet("paid")]
        public async Task<ActionResult<PaidInvoicesReportModel>> GetPaid(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var query = new GetPaidInvoicesReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate 
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
