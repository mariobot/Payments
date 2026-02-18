using Hyip_Payments.Command.InvoiceCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.InvoiceQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Invoice
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Disabled: Server-side Blazor components run in authenticated context
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
            var result = await _mediator.Send(new EditInvoiceCommand(invoice));
            if (result == null)
               return NotFound();
            return NoContent();
        }

        // DELETE: api/Invoice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // You would implement and use a DeleteInvoiceCommand here
            var result = await _mediator.Send(new DeleteInvoiceCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }

        // POST: api/Invoice/with-products
        /// <summary>
        /// Create invoice with products in a single transaction
        /// </summary>
        [HttpPost("with-products")]
        public async Task<ActionResult<InvoiceWithItemsDto>> CreateWithProducts([FromBody] AddInvoiceWithProductsCommand command)
        {
            // Set the current user as the creator
            command.CreatedByUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetWithItems), new { id = result.InvoiceId }, result);
        }

        // GET: api/Invoice/5/with-items
        /// <summary>
        /// Get invoice with all its items (products)
        /// </summary>
        [HttpGet("{id}/with-items")]
        public async Task<ActionResult<InvoiceWithItemsResponse>> GetWithItems(int id)
        {
            var result = await _mediator.Send(new GetInvoiceWithItemsQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // PUT: api/Invoice/5/with-items
        /// <summary>
        /// Update invoice with items in a single transaction
        /// </summary>
        [HttpPut("{id}/with-items")]
        public async Task<ActionResult<InvoiceWithItemsDto>> UpdateWithItems(int id, [FromBody] UpdateInvoiceWithItemsCommand command)
        {
            command.InvoiceId = id; // Ensure ID from route is used
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
