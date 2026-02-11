using Hyip_Payments.Command.ProductCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.ProductQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<List<ProductModel>>> GetAll()
        {
            var products = await _mediator.Send(new GetProductListQuery());
            return Ok(products);
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> Get(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductModel>> Create([FromBody] ProductModel product)
        {
            var result = await _mediator.Send(new AddProductCommand(product));
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductModel>> Edit(int id, [FromBody] ProductModel product)
        {
            if (id != product.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditProductCommand(product));
            return Ok(result);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }

        // GET: api/Product/for-selection
        /// <summary>
        /// Get simplified product list for selection dropdowns
        /// </summary>
        [HttpGet("for-selection")]
        public async Task<ActionResult<List<ProductSelectionDto>>> GetForSelection()
        {
            var products = await _mediator.Send(new GetProductListForSelectionQuery());
            return Ok(products);
        }
    }
}
