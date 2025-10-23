using Hyip_Payments.Command.CategoryCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CategoryQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Category
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<List<CategoryModel>>> GetAll()
        {
            var categories = await _mediator.Send(new GetCategoriesListQuery());
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryModel>> Get(int id)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<CategoryModel>> Create([FromBody] CategoryModel category)
        {
            var result = await _mediator.Send(new AddCategoryCommand(category));
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryModel>> Edit(int id, [FromBody] CategoryModel category)
        {
            if (id != category.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditCategoryCommand(category));
            return Ok(result);
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
