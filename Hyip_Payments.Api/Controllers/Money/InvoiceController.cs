using Hyip_Payments.Models;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Query.InvoiceQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Money
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Invoice
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _mediator.Send(new GetAllInvoicesQuery());
            return Ok(invoices);
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _mediator.Send(new GetInvoiceByIdQuery(id));
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }

        // POST: api/Invoice
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceModel model)
        {
            var result = await _mediator.Send(new AddInvoiceCommand(model));
            return CreatedAtAction(nameof(Details), new { id = result.Id }, result);
        }

        // PUT: api/Invoice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] InvoiceModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditInvoiceCommand(model));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteInvoiceCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
