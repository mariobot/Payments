using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hyip_Payments.Api.Controllers.User
{
    [ApiController]
    [Route("api/user/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST: api/user/register
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserModel>> Register([FromBody] UserApplicationModel userApp)
        {
            // Map UserApplicationModel to RegisterUserCommand
            var command = new RegisterUserCommand
            {
                Email = userApp.Email,
                Password = userApp.Password,
                Username = userApp.Email, // Or use another property if you have Username
                EmailConfirmed = userApp.EmailConfirmed
            };

            var user = await _mediator.Send(command);
            if (user == null)
                return BadRequest("Registration failed.");
            return Ok(user);
        }
    }
}
