using System.Threading.Tasks;
using Hyip_Payments.Models;
using Hyip_Payments.Command.BrandCommand;
using Hyip_Payments.Query.BrandQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Brand
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Brand
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _mediator.Send(new GetBrandListQuery());
            return Ok(brands);
        }

        // GET: api/Brand/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brand = await _mediator.Send(new GetBrandByIdQuery(id));
            if (brand == null)
                return NotFound();
            return Ok(brand);
        }

        // POST: api/Brand
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BrandModel brand)
        {
            var result = await _mediator.Send(new AddBrandCommand(brand));
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/Brand/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] BrandModel brand)
        {
            if (id != brand.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditBrandCommand(brand));
            return Ok(result);
        }

        // DELETE: api/Brand/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteBrandCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
