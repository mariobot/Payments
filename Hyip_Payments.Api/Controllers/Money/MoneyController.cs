using Hyip_Payments.Command.MoneyCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.MoneyQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Money
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MoneyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MoneyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Money
        [HttpGet]
        public async Task<ActionResult<List<MoneyModel>>> GetAll()
        {
            try
            {
                var moneyList = await _mediator.Send(new GetMoneyQuery());
                return Ok(moneyList);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: api/Money/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MoneyModel>> Get(int id)
        {
            var money = await _mediator.Send(new GetMoneyByIdQuery(id));
            if (money == null)
                return NotFound();
            return Ok(money);
        }

        // POST: api/Money
        [HttpPost]
        public async Task<ActionResult<MoneyModel>> Create([FromBody] MoneyModel money)
        {
            var result = await _mediator.Send(new AddMoneyCommand(money));
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/Money/5
        [HttpPut("{id}")]
        public async Task<ActionResult<MoneyModel>> Edit(int id, [FromBody] MoneyModel money)
        {
            if (id != money.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditMoneyCommand(money));
            return Ok(result);
        }

        // DELETE: api/Money/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteMoneyCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
