using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Roles.CommandHandler;
using Csdsa.Application.Services.EntityServices.Roles.Queries;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Application.Users.Queries;
using Csdsa.Domain.Models.Common;
using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.EntityControllers
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
            try
            {
                var result = await _mediator.Send(
                    new CreateUserCommand(request.UserName, request.Email, request.Password)
                );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<UserDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpGet("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByEmailQuery(email));
                return user == null ? NotFound() : Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpPost("SoftDeleteUser")]
        public async Task<IActionResult> SoftDeleteUser([FromBody] string username)
        {
            try
            {
                var user = await _mediator.Send(new SoftDeleteUserCommand(username));
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _mediator.Send(new GetAllUsersQuery());
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<UserDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _mediator.Send(new GetUserByIdQuery(id));
                if (user == null)
                    return NotFound(ApiResponse<UserDto>.ErrorResult("User not found."));
                return Ok(ApiResponse<UserDto>.SuccessResult(user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<UserDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpPut("UpdateExistingUser")]
        public async Task<IActionResult> UpdateExistingUser(
            Guid id,
            [FromBody] UpdateUserCommand cmd
        )
        {
            if (id != cmd.userId)
                return BadRequest("ID in URL does not match ID in body.");

            try
            {
                await _mediator.Send(cmd);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<bool>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUserQuery()
        {
            try
            {
                var result = await _mediator.Send(new GetCurrentUserQuery());
                return Ok(ApiResponse<UserDto>.SuccessResult(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<UserDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpPost("ActivateUserAccount")]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            try
            {
                var success = await _mediator.Send(new ActivateUserAccountCommand(id));

                return Ok(ApiResponse<bool>.SuccessResult(success, "User activated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<bool>.ErrorResult("Activation failed", ex.Message)
                );
            }
        }

        [HttpPost("DeactivateUserAccount")]
        public async Task<IActionResult> DeactivateUserAccount(Guid userId)
        {
            try
            {
                var success = await _mediator.Send(new ActivateUserAccountCommand(userId));

                return Ok(
                    ApiResponse<bool>.SuccessResult(success, "User deactivated successfully")
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<bool>.ErrorResult("Deactivation failed", ex.Message)
                );
            }
        }

        [HttpGet("AssignRoleToUser")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetRolesAssignedToUser(Guid id)
        {
            try
            {
                var role = await _mediator.Send(new GetUserRoleQuery(id));
                return Ok(ApiResponse<RoleDto>.SuccessResult(role));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<RoleDto>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpPost("GetRolesAssignedToUser")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> AssignRoleToUser(
            Guid id,
            [FromBody] AssignRoleToUserCommand request
        )
        {
            if (id != request.UserId)
                return BadRequest("ID in URL does not match ID in request body.");

            try
            {
                var result = await _mediator.Send(
                    new AssignRoleToUserCommand(request.UserId, request.Role)
                );
                return Ok(ApiResponse<bool>.SuccessResult(result, "Role assigned successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<bool>.ErrorResult("Role assignment failed", ex.Message)
                );
            }
        }
    }
}
