using Hyip_Payments.Command.InvoiceItemCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.InvoiceItemQuery;
using Hyip_Payments.Query.PaymentQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.InvoiceItem
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoiceItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/InvoiceItem
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var items = await _mediator.Send(new GetInvoiceItemListQuery());
            return Ok(items);
        }

        // GET: api/InvoiceItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _mediator.Send(new GetInvoiceItemByIdQuery(id));
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        // POST: api/InvoiceItem
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceItemModel model)
        {
            var result = await _mediator.Send(new AddInvoiceItemCommand(model));
            return CreatedAtAction(nameof(Details), new { id = result.Id }, result);
        }

        // PUT: api/InvoiceItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] InvoiceItemModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditInvoiceItemCommand(model));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/InvoiceItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteInvoiceItemCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
