using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly TokenService _tokenService;

        public AuthController(IMediator mediator, TokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var user = await _mediator.Send(command);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Get user roles
            var roles = user.UserRoles?.Select(ur => ur.Role?.Name ?? "User").ToList() ?? new List<string>();

            // Generate JWT token
            var token = _tokenService.GenerateToken(user.Id, user.Username, user.Email, roles);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    roles
                }
            });
        }
    }

    public record LoginRequest(string Email, string Password);
}
