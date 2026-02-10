using Hyip_Payments.Command.UserCommand;
using Hyip_Payments.Query.UserQuery;
using Hyip_Payments.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _mediator.Send(new GetUserListQuery());
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new AddUserCommand(model));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] UserModel model)
        {
            if (id != model.Id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(new EditUserCommand(model));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteUserCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }

        // GET: api/User/username/{username}
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var user = await _mediator.Send(new GetUserByUsernameQuery(username));
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // GET: api/User/email/{email}
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(email));
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // PUT: api/User/toggle-active/5
        [HttpPut("toggle-active/{id}")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var success = await _mediator.Send(new ToggleUserActiveCommand(id));
            if (!success)
                return NotFound();
            return NoContent();
        }

        // PUT: api/User/change-password/5
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("New password is required");

            var success = await _mediator.Send(new ChangePasswordCommand(id, request.NewPassword));
            if (!success)
                return NotFound();
            return NoContent();
        }
    }

    // Request model for password change
    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
