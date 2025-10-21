using System.Collections.Generic;
using System.Threading.Tasks;
using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.InvoiceQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Invoice
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
        public async Task<ActionResult<IEnumerable<InvoiceModel>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllInvoicesQuery());
            return Ok(result);
        }

        // GET: api/Invoice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceModel>> GetById(int id)
        {
            var result = await _mediator.Send(new GetInvoiceByIdQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/Invoice
        [HttpPost]
        public async Task<ActionResult<InvoiceModel>> Create([FromBody] InvoiceModel invoice)
        {
            var result = await _mediator.Send(new AddInvoiceCommand(invoice));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Invoice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] InvoiceModel invoice)
        {
            invoice.Id = id;
            // You would implement and use an EditInvoiceCommand here
            // var result = await _mediator.Send(new EditInvoiceCommand(invoice));
            // if (result == null)
            //     return NotFound();
            // return NoContent();
            return NoContent(); // Placeholder
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // You would implement and use a DeleteInvoiceCommand here
            // var result = await _mediator.Send(new DeleteInvoiceCommand(id));
            // if (!result)
            //     return NotFound();
            // return NoContent();
            return NoContent(); // Placeholder
        }
    }
}
