using Csdsa.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.EntityControllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseController
{
    public UserController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        : base(unitOfWork, logger, mediator) { }

    // [HttpGet("GetUserByEmail")]
    // public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    // {
    //     try
    //     {
    //         var user = await _mediator.Send(new GetUserByEmailQuery(email));
    //         return user == null ? NotFound() : Ok(user);
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<UserDto>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpPost("SoftDeleteUser")]
    // public async Task<IActionResult> SoftDeleteUser([FromBody] string username)
    // {
    //     try
    //     {
    //         var user = await _mediator.Send(new SoftDeleteUserCommand(username));
    //         return Ok(user);
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<UserDto>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("GetAllUsers")]
    // public async Task<IActionResult> GetAllUsers()
    // {
    //     try
    //     {
    //         var users = await _mediator.Send(new GetAllUsersQuery());
    //         return Ok(users);
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<UserDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("GetUserById")]
    // public async Task<IActionResult> GetById(Guid id)
    // {
    //     try
    //     {
    //         var user = await _mediator.Send(new GetUserByIdQuery(id));
    //         if (user == null)
    //             return NotFound(OperationResult<UserDto>.ErrorResult("User not found."));
    //         return Ok(OperationResult<UserDto>.SuccessResult(user));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<UserDto>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<UserDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpPut("UpdateExistingUser")]
    // public async Task<IActionResult> UpdateExistingUser(Guid id, [FromBody] UpdateUserCommand cmd)
    // {
    //     if (id != cmd.userId)
    //         return BadRequest("ID in URL does not match ID in body.");
    //
    //     try
    //     {
    //         await _mediator.Send(cmd);
    //         return NoContent();
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, OperationResult<bool>.ErrorResult("Unexpected error.", ex.Message));
    //     }
    // }

    // [HttpGet("GetCurrentUser")]
    // public async Task<IActionResult> GetCurrentUserQuery()
    // {
    //     try
    //     {
    //         var result = await _mediator.Send(new GetCurrentUserQuery());
    //         return Ok(OperationResult<UserDto>.SuccessResult(result));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<UserDto>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<UserDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpPost("ActivateUserAccount")]
    // public async Task<IActionResult> ActivateUser(Guid id)
    // {
    //     try
    //     {
    //         var success = await _mediator.Send(new ActivateUserAccountCommand(id));
    //
    //         return Ok(OperationResult<bool>.SuccessResult(success, "User activated successfully"));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(500, OperationResult<bool>.ErrorResult("Activation failed", ex.Message));
    //     }
    // }

    // [HttpPost("DeactivateUserAccount")]
    // public async Task<IActionResult> DeactivateUserAccount(Guid userId)
    // {
    //     try
    //     {
    //         var success = await _mediator.Send(new ActivateUserAccountCommand(userId));
    //
    //         return Ok(OperationResult<bool>.SuccessResult(success, "User deactivated successfully"));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<bool>.ErrorResult("Deactivation failed", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("AssignRoleToUser")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> GetRolesAssignedToUser(Guid id)
    // {
    //     try
    //     {
    //         var role = await _mediator.Send(new GetUserRoleQuery(id));
    //         return Ok(OperationResult<RoleDto>.SuccessResult(role));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<RoleDto>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpPost("GetRolesAssignedToUser")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> AssignRoleToUser(
    //     Guid id,
    //     [FromBody] AssignRoleToUserCommand request
    // )
    // {
    //     if (id != request.UserId)
    //         return BadRequest("ID in URL does not match ID in request body.");
    //
    //     try
    //     {
    //         var result = await _mediator.Send(
    //             new AssignRoleToUserCommand(request.UserId, request.Role)
    //         );
    //         return Ok(ApiResponse<bool>.SuccessResult(result, "Role assigned successfully"));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return BadRequest(ApiResponse<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             ApiResponse<bool>.ErrorResult("Role assignment failed", ex.Message)
    //         );
    //     }
    // }
}
