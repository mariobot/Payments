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
    }
}
