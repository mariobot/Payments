using Hyip_Payments.Command.CoinCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CoinQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Coin
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class CoinController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoinController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Coin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoinModel>>> GetAll()
        {
            var result = await _mediator.Send(new GetCoinsQuery());
            return Ok(result);
        }

        // GET: api/Coin/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoinModel>> GetById(int id)
        {
            var result = await _mediator.Send(new GetCointByIdQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/Coin
        [HttpPost]
        public async Task<ActionResult<CoinModel>> Create([FromBody] CoinModel coin)
        {
            var result = await _mediator.Send(new AddCoinCommand(coin));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Coin/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CoinModel coin)
        {
            coin.Id = id;
            var result = await _mediator.Send(new EditCoinCommand(coin));
            if (result == null)
                return NotFound();
            return NoContent();
        }

        // DELETE: api/Coin/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCoinCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
