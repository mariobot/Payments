using Hyip_Payments.Models.Reports;
using Hyip_Payments.Query.ReportQuery.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentReportController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentReportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/PaymentReport/summary?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get payment summary report for a specific date range
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<PaymentSummaryReportModel>> GetSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var query = new GetPaymentSummaryReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate 
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/PaymentReport/methods?startDate=2024-01-01&endDate=2024-12-31
        /// <summary>
        /// Get payment methods analysis report for a specific date range
        /// </summary>
        [HttpGet("methods")]
        public async Task<ActionResult<PaymentMethodsReportModel>> GetMethods(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var query = new GetPaymentMethodsReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate 
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/PaymentReport/transactions?startDate=2024-01-01&endDate=2024-12-31&status=Completed
        /// <summary>
        /// Get transaction log report with optional filters
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult<TransactionLogReportModel>> GetTransactions(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate,
            [FromQuery] string? status = null,
            [FromQuery] string? paymentMethod = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetTransactionLogReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                Status = status,
                PaymentMethod = paymentMethod,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/PaymentReport/status?startDate=2024-01-01&endDate=2024-12-31&statusFilter=Completed
        /// <summary>
        /// Get payment status report with breakdown of Pending, Completed, Failed, and Cancelled payments
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<PaymentStatusReportModel>> GetStatus(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate,
            [FromQuery] string? statusFilter = null)
        {
            var query = new GetPaymentStatusReportQuery 
            { 
                StartDate = startDate, 
                EndDate = endDate,
                StatusFilter = statusFilter
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
