using Hyip_Payments.Command.PaymentCommand;
using Hyip_Payments.Query.PaymentQuery;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Payment
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Payment
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var payments = await _mediator.Send(new GetPaymentListQuery());
            return Ok(payments);
        }

        // GET: api/Payment/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var payment = await _mediator.Send(new GetPaymentByIdQuery(id));
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        // POST: api/Payment
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentModel model)
        {
            var result = await _mediator.Send(new AddPaymentCommand(model));
            return CreatedAtAction(nameof(Details), new { id = result.Id }, result);
        }

        // PUT: api/Payment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(Guid id, [FromBody] PaymentModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditPaymentCommand(model));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/Payment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _mediator.Send(new DeletePaymentCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }

        // POST: api/Payment/transaction
        /// <summary>
        /// Create a payment transaction
        /// </summary>
        [HttpPost("transaction")]
        public async Task<ActionResult<PaymentTransactionModel>> CreateTransaction([FromBody] AddPaymentTransactionCommand command)
        {
            // Set the current user as the processor
            command.ProcessedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTransaction), new { id = result.Id }, result);
        }

        // GET: api/Payment/transactions
        /// <summary>
        /// Get all payment transactions
        /// </summary>
        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<PaymentTransactionModel>>> GetAllTransactions()
        {
            var transactions = await _mediator.Send(new GetPaymentTransactionListQuery());
            return Ok(transactions);
        }

        // GET: api/Payment/transaction/5
        /// <summary>
        /// Get payment transaction by ID
        /// </summary>
        [HttpGet("transaction/{id}")]
        public async Task<ActionResult<PaymentTransactionModel>> GetTransaction(int id)
        {
            var transaction = await _mediator.Send(new GetPaymentTransactionByIdQuery(id));
            if (transaction == null)
                return NotFound();
            return Ok(transaction);
        }

        // PUT: api/Payment/transaction/5
        /// <summary>
        /// Update payment transaction
        /// </summary>
        [HttpPut("transaction/{id}")]
        public async Task<ActionResult<PaymentTransactionModel>> UpdateTransaction(int id, [FromBody] EditPaymentTransactionCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/Payment/transaction/5
        /// <summary>
        /// Delete payment transaction
        /// </summary>
        [HttpDelete("transaction/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var success = await _mediator.Send(new DeletePaymentTransactionCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
