using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hyip_Payments.Command.CountryCommand;
using Hyip_Payments.Query.CountryQuery;
using Hyip_Payments.Models;

namespace Hyip_Payments.Api.Controllers.Country
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Temporarily commented for testing
    public class CountryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryModel>>> GetAll()
        {
            var result = await _mediator.Send(new GetCountryListQuery());
            return Ok(result);
        }

        // GET: api/Country/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryModel>> GetById(int id)
        {
            var result = await _mediator.Send(new GetCountryByIdQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/Country
        [HttpPost]
        public async Task<ActionResult<CountryModel>> Create([FromBody] CountryModel country)
        {
            var result = await _mediator.Send(new AddCountryCommand(country));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Country/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CountryModel country)
        {
            country.Id = id;
            var result = await _mediator.Send(new EditCountryCommand(country));
            if (result == null)
                return NotFound();
            return NoContent();
        }

        // DELETE: api/Country/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCountryCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
