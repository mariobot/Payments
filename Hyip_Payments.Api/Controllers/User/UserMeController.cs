using Hyip_Payments.Models;
using Hyip_Payments.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hyip_Payments.Api.Controllers.User
{
    [Route("api/user")]
    [ApiController]
    public class UserMeController : ControllerBase
    {
        private readonly PaymentsDbContext _context;
        private readonly ILogger<UserMeController> _logger;

        public UserMeController(PaymentsDbContext context, ILogger<UserMeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get user by email or username
        /// Since the API is separate from the Web app, we can't rely on cookies.
        /// The client should call this with the email/username from the authentication state.
        /// </summary>
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<UserModel>> GetByEmail(string email)
        {
            _logger.LogInformation($"GetByEmail called with email: {email}");

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                _logger.LogWarning($"User not found with email: {email}");
                return NotFound(new { message = $"User not found with email: {email}" });
            }

            _logger.LogInformation($"User found: {user.Id} - {user.Username}");
            return Ok(user);
        }

        /// <summary>
        /// Get user by username
        /// </summary>
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<UserModel>> GetByUsername(string username)
        {
            _logger.LogInformation($"GetByUsername called with username: {username}");

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new { message = "Username is required" });
            }

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                _logger.LogWarning($"User not found with username: {username}");
                return NotFound(new { message = $"User not found with username: {username}" });
            }

            _logger.LogInformation($"User found: {user.Id} - {user.Username}");
            return Ok(user);
        }

        /// <summary>
        /// Debug endpoint to see all current user claims (if any)
        /// This will show if cookies are being sent from the Web app
        /// </summary>
        [HttpGet("debug-claims")]
        public IActionResult GetClaims()
        {
            _logger.LogInformation("=== Debug Claims Called ===");

            var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            _logger.LogInformation($"IsAuthenticated: {isAuthenticated}");

            if (!isAuthenticated)
            {
                return Ok(new
                {
                    IsAuthenticated = false,
                    Message = "No authentication found. This is expected when API and Web are on different origins.",
                    Recommendation = "Use the /by-email or /by-username endpoints instead of relying on cookies."
                });
            }

            var claims = User.Claims.Select(c => new 
            { 
                Type = c.Type,
                Value = c.Value,
                ShortType = c.Type.Split('/').Last()
            }).ToList();

            return Ok(new
            {
                IsAuthenticated = true,
                Name = User.Identity?.Name,
                AuthenticationType = User.Identity?.AuthenticationType,
                Claims = claims,
                ClaimCount = claims.Count
            });
        }

        /// <summary>
        /// Legacy endpoint - kept for backwards compatibility
        /// Returns 501 Not Implemented with instructions
        /// </summary>
        [HttpGet("me")]
        public IActionResult GetMe()
        {
            _logger.LogWarning("Legacy /me endpoint called");

            return StatusCode(501, new
            {
                message = "The /me endpoint doesn't work with separate API/Web architecture.",
                solution = "Use /api/user/by-email/{email} or /api/user/by-username/{username} instead.",
                example = "GET /api/user/by-email/john@example.com",
                note = "Get the email/username from the Blazor authentication state and pass it to the API."
            });
        }
    }
}
