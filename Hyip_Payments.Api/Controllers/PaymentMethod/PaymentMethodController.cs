using Hyip_Payments.Command.PaymentMethodCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.PaymentMethodQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.PaymentMethod
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentMethodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/PaymentMethod
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentMethodModel>>> GetAll()
        {
            var paymentMethods = await _mediator.Send(new GetPaymentMethodListQuery());
            return Ok(paymentMethods);
        }

        // GET: api/PaymentMethod/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodModel>> GetById(int id)
        {
            var paymentMethod = await _mediator.Send(new GetPaymentMethodByIdQuery(id));
            if (paymentMethod == null)
                return NotFound();
            return Ok(paymentMethod);
        }

        // POST: api/PaymentMethod
        [HttpPost]
        public async Task<ActionResult<PaymentMethodModel>> Create([FromBody] PaymentMethodModel paymentMethod)
        {
            var result = await _mediator.Send(new AddPaymentMethodCommand(paymentMethod));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/PaymentMethod/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentMethodModel>> Edit(int id, [FromBody] PaymentMethodModel paymentMethod)
        {
            if (id != paymentMethod.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(new EditPaymentMethodCommand(paymentMethod));
            if (result == null)
                return NotFound();
            
            return Ok(result);
        }

        // DELETE: api/PaymentMethod/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeletePaymentMethodCommand(id));
            if (!result)
                return NotFound();
            
            return NoContent();
        }
    }
}
