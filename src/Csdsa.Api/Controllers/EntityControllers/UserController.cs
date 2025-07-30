using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Application.Users.Queries;
using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
            : base(unitOfWork, logger, mediator) { }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _mediator.Send(
                new CreateUserCommand(request.UserName, request.Email, request.Password)
            );
            return Ok(result);
        }

        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(email));
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost("SoftDeleteUser")]
        public async Task<IActionResult> SoftDeleteUser([FromBody] string username)
        {
            var user = await _mediator.Send(new SoftDeleteUserCommand(username));
            return Ok(user);
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _mediator.Send(new GetAllUsersQuery());
            return Ok(users);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById([FromBody] Guid id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(id));
            return Ok(user);
        }

        [HttpPut("UpdateExistingUser")]
        public async Task<IActionResult> UpdateExistingUser(
            Guid id,
            [FromBody] UpdateUserCommand cmd
        )
        {
            if (id != cmd.userId)
                return BadRequest("ID in URL does not match ID in body.");

            await _mediator.Send(cmd);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrent()
        {
            var result = await _mediator.Send(new GetCurrentUserQuery());
            return Ok(result);
        }

        [HttpPost("ActivateUserAccount")]
        public async Task<IActionResult> ActivateUserAccount(Guid userId)
        {
            var result = await _mediator.Send(new ActivateUserAccountCommand(userId));
            return Ok(result);
        }

        [HttpPost("DeactivateUserAccount")]
        public async Task<IActionResult> DeactivateUserAccount(Guid userId)
        {
            var result = await _mediator.Send(new ActivateUserAccountCommand(userId));
            return Ok(result);
        }

        // POST: /api/users/{id}/roles => AssignRoleToUser

        // GET: /api/users/{id}/roles => GetRolesAssignedToUser
    }
}
