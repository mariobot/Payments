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
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _mediator.Send(new GetUserListQuery());
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
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
            var result = await _mediator.Send(new AddUserCommand(model));
            return CreatedAtAction(nameof(Details), new { id = result.Id }, result);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] UserModel model)
        {
            if (id != model.Id)
                return BadRequest();

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
    }
}
