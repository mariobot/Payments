using Hyip_Payments.Models;
using Hyip_Payments.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hyip_Payments.Api.Controllers.Wallet
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly PaymentsDbContext _context;

        public WalletController(PaymentsDbContext context)
        {
            _context = context;
        }

        // GET: api/Wallet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WalletModel>>> GetAll()
        {
            return Ok(await _context.Wallets.ToListAsync());
        }

        // GET: api/Wallet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WalletModel>> GetById(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
                return NotFound();
            return Ok(wallet);
        }

        // POST: api/Wallet
        [HttpPost]
        public async Task<ActionResult<WalletModel>> Create([FromBody] WalletModel wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = wallet.Id }, wallet);
        }

        // PUT: api/Wallet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] WalletModel wallet)
        {
            if (id != wallet.Id)
                return BadRequest();

            _context.Entry(wallet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Wallets.AnyAsync(w => w.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Wallet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet == null)
                return NotFound();

            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
