using Csdsa.Api.Controllers.Base;
using Csdsa.Application.DTOs.Entities.Role;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.EntityServices.Roles.Request;
using Csdsa.Application.Services.EntityServices.Roles.RequestHandlers;
using Csdsa.Domain.Models;
using Csdsa.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.EntityControllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : BaseController
{
    public RoleController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        : base(unitOfWork, logger, mediator) { }

    [HttpGet("GetAllRoles")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(ApiResponse<List<RoleDto>>.SuccessResult(roles));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<List<RoleDto>>.ErrorResult("Unexpected error.", ex.Message)
            );
        }
    }

    [HttpGet("GetRoleById")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetRoleById(int id)
    {
        try
        {
            var role = await _mediator.Send(new GetRoleByIdQuery(id));
            if (role == null)
                return NotFound(ApiResponse<RoleDto>.ErrorResult("Role not found."));

            return Ok(ApiResponse<RoleDto>.SuccessResult(role));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
            );
        }
    }

    [HttpGet("GetRoleByName")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetRoleByName(string roleName)
    {
        try
        {
            var role = await _mediator.Send(new GetRoleByNameQuery(roleName));
            if (role == null)
                return NotFound(ApiResponse<RoleDto>.ErrorResult("Role not found."));

            return Ok(ApiResponse<RoleDto>.SuccessResult(role));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<RoleDto>.ErrorResult("Unexpected error.", ex.Message)
            );
        }
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommand request)
    {
        try
        {
            var result = await _mediator.Send(
                new AssignRoleToUserCommand(request.UserIds, request.Role)
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

    [HttpPost("RemoveRoleFromUser")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromBody] RemoveRoleFromUserCommand request
    )
    {
        try
        {
            var result = await _mediator.Send(new RemoveRoleFromUserCommand(request.UserId));
            return Ok(ApiResponse<bool>.SuccessResult(result, "Role removed successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<bool>.ErrorResult("Role removal failed", ex.Message)
            );
        }
    }

    [HttpGet("{roleId}/users")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetUsersByRole(int roleId)
    {
        try
        {
            var users = await _mediator.Send(new GetUsersByRoleQuery((UserRole)roleId));
            return Ok(ApiResponse<List<UserDto>>.SuccessResult(users));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<List<UserDto>>.ErrorResult("Unexpected error.", ex.Message)
            );
        }
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetUserRole(Guid userId)
    {
        try
        {
            var role = await _mediator.Send(new GetUserRoleQuery(userId));
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

    [HttpPost("AssignRole")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AssignRoles([FromBody] AssignRoleToUserCommand request)
    {
        try
        {
            var result = await _mediator.Send(
                new AssignRolesCommand(request.UserIds, request.Role)
            );
            return Ok(
                ApiResponse<AssignRolesResult>.SuccessResult(result, "Role assignment completed")
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                ApiResponse<AssignRolesResult>.ErrorResult("Role assignment failed", ex.Message)
            );
        }
    }
}
