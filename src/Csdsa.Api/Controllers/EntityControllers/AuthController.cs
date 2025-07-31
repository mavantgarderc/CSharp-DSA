using Application.Users.Commands;
using Csdsa.Api.Controllers.Base;
using Csdsa.Application.Common.Interfaces;
using Csdsa.Application.DTOs.Entities.User;
using Csdsa.Application.Services.EntityServices.Users.Requests;
using Csdsa.Application.Users.Queries;
using Csdsa.Domain.Models.Common;
using Csdsa.Domain.ViewModel.EntityViewModel.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Csdsa.Api.Controllers.EntityControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        public AuthController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
            : base(unitOfWork, logger, mediator) { }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(CreateUserCommand cmd)
        {
            try
            {
                var user = await _mediator.Send(cmd);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ApiResponse<UserDto>.ErrorResult("Unexpected error.", ex.Message)
                );
            }
        }

        [HttpGet("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            try
            {
                var result = await _mediator.Send(
                    new LoginUserCommand(request.Email, request.Password)
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

        // POST: /api/auth/logout => LogOut
        // POST: /api/auth/refresh-token => RefreshToken
        // POST: /api/auth/reset-passwoord => ResetPassword
        // GET: /api/auth/sessions => GetCurrentUserSessions
        // DELETE: /api/auth/session/{id} => KillSession

        // GET: /api/audit-log => ViewAuditLogs
        // GET: /api/audit-log/{id} => ViewAuditLog
    }
}
