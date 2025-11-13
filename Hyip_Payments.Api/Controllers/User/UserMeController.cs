using Hyip_Payments.Models;
using Hyip_Payments.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hyip_Payments.Api.Controllers.User
{
    [Route("api/user/me")]
    [ApiController]
    //[Authorize]
    public class UserMeController : ControllerBase
    {
        private readonly PaymentsDbContext _context;

        public UserMeController(PaymentsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<UserModel>> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
    }
}
