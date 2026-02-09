using Hyip_Payments.Models;
using Hyip_Payments.Command.WalletCommand;
using Hyip_Payments.Query.WalletQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Wallet
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WalletController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Wallet
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var wallets = await _mediator.Send(new GetWalletListQuery());
            return Ok(wallets);
        }

        // GET: api/Wallet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletModel?>> GetById(int id)
        {
            var result = await _mediator.Send(new GetWalletQuery(id));
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // POST: api/Wallet
        [HttpPost]
        public async Task<ActionResult<WalletModel>> Create([FromBody] WalletModel wallet)
        {
            var result = await _mediator.Send(new AddWalletCommand(wallet));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Wallet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] WalletModel wallet)
        {
            if (id != wallet.Id)
                return BadRequest();

            var result = await _mediator.Send(new EditWalletCommand(wallet));
            if (result == null)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/Wallet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // You may want to implement a DeleteWalletCommand
            return StatusCode(501, "DeleteWalletCommand not implemented.");
        }

        // PUT: api/Wallet/disable/5
        [HttpPut("disable/{id}")]
        public async Task<IActionResult> Disable(int id)
        {
            var result = await _mediator.Send(new DisableWalletCommand(id));
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
