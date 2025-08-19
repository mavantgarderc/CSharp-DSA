using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.Auth.GetAllRoles;
using Csdsa.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.AuthControllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : BaseController
{
    public RoleController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        : base(unitOfWork, logger, mediator) { }

    /// <summary>
    /// retrieves all roles in the system with optional filtering and inclusion options.
    /// returns a list of roles with basic information including user counts and creation details.
    /// </summary>
    /// <param name="includeInactive">optional parameter to include inactive/soft-deleted roles in the results</param>
    /// <param name="includePermissions">optional parameter to include role permissions in the response</param>
    /// <returns>
    /// returns an OperationResult containing a list of RoleDto objects.
    /// </returns>
    [HttpGet("GetAllRoles")]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] bool includeInactive = false,
        [FromQuery] bool includePermissions = false
    )
    {
        try
        {
            var query = new GetAllRolesQuery
            {
                IncludeInactive = includeInactive,
                IncludePermissions = includePermissions,
            };

            var result = await _mediator.Send(query);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                OperationResult<List<RoleDto>>.ErrorResult(
                    "An unexpected error occurred while retrieving roles.",
                    ex.Message
                )
            );
        }
    }

    // [HttpGet("GetRoleById")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> GetRoleById(int id)
    // {
    //     try
    //     {
    //         var role = await _mediator.Send(new GetRoleByIdQuery(id));
    //         if (role == null)
    //             return NotFound(OperationResult<RoleDto>.ErrorResult("Role not found."));
    //
    //         return Ok(OperationResult<RoleDto>.SuccessResult(role));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("GetRoleByName")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> GetRoleByName(string roleName)
    // {
    //     try
    //     {
    //         var role = await _mediator.Send(new GetRoleByNameQuery(roleName));
    //         if (role == null)
    //             return NotFound(OperationResult<RoleDto>.ErrorResult("Role not found."));
    //
    //         return Ok(OperationResult<RoleDto>.SuccessResult(role));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpPost("assign-role")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand request)
    // {
    //     try
    //     {
    //         var result = await _mediator.Send(
    //             new AssignRoleToUserCommand(request.UserIds, request.Role)
    //         );
    //         return Ok(OperationResult<bool>.SuccessResult(result, "Role assigned successfully"));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return BadRequest(OperationResult<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<bool>.ErrorResult("Role assignment failed", ex.Message)
    //         );
    //     }
    // }

    // [HttpPost("RemoveRoleFromUser")]
    // [Authorize(Roles = "SuperAdmin")]
    // public async Task<IActionResult> RemoveRoleFromUser(
    //     [FromBody] RemoveRoleFromUserCommand request
    // )
    // {
    //     try
    //     {
    //         var result = await _mediator.Send(new RemoveRoleFromUserCommand(request.UserId));
    //         return Ok(OperationResult<bool>.SuccessResult(result, "Role removed successfully"));
    //     }
    //     catch (KeyNotFoundException ex)
    //     {
    //         return NotFound(OperationResult<bool>.ErrorResult(ex.Message));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<bool>.ErrorResult("Role removal failed", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("{roleId}/users")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> GetUsersByRole(int roleId)
    // {
    //     try
    //     {
    //         var users = await _mediator.Send(new GetUsersByRoleQuery((UserRole)roleId));
    //         return Ok(OperationResult<List<UserDto>>.SuccessResult(users));
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
    //         );
    //     }
    // }

    // [HttpGet("user/{userId}")]
    // [Authorize(Roles = "Admin,SuperAdmin")]
    // public async Task<IActionResult> GetUserRole(Guid userId)
    // {
    //     try
    //     {
    //         var role = await _mediator.Send(new GetUserRoleQuery(userId));
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

    // [HttpPost("AssignRole")]
    // [Authorize(Roles = "SuperAdmin")]
    // public async Task<IActionResult> AssignRoles([FromBody] AssignRoleToUserCommand request)
    // {
    //     try
    //     {
    //         var result = await _mediator.Send(
    //             new AssignRolesCommand(request.UserIds, request.Role)
    //         );
    //         return Ok(
    //             OperationResult<AssignRolesResult>.SuccessResult(result, "Role assignment completed")
    //         );
    //     }
    //     catch (Exception ex)
    //     {
    //         return StatusCode(
    //             500,
    //             OperationResult<AssignRolesResult>.ErrorResult("Role assignment failed", ex.Message)
    //         );
    //     }
    // }
}
