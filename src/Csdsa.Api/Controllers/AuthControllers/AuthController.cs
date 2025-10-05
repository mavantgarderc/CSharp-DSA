using Csdsa.Api.DTOs.Auth;
using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.Auth.GetUserProfile;
using Csdsa.Application.Services.Auth.Login;
using Csdsa.Application.Services.Auth.Logout;
using Csdsa.Application.Services.Auth.Register;
using Csdsa.Application.Services.Auth.SoftDeleteUser;
using Csdsa.Domain.Exceptions;
using Csdsa.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Csdsa.Api.Controllers.AuthControllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController : BaseController
{
    public AuthController(IUnitOfWork unitOfWork, ILogger logger, IMediator mediator)
        : base(unitOfWork, logger, mediator) { }

    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication response with JWT tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(OperationResult<AuthResponse>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 423)]
    public async Task<ActionResult<OperationResult<AuthResponse>>> Login(
        [FromBody] LoginRequest request
    )
    {
        try
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GetClientIpAddress(),
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return BadRequest(CreateProblemDetails("Invalid Credentials", ex.Message, 400));
        }
        catch (AccountLockedException ex)
        {
            return StatusCode(423, CreateProblemDetails("Account Locked", ex.Message, 423));
        }
        catch (EmailNotVerifiedException ex)
        {
            return BadRequest(CreateProblemDetails("Email Not Verified", ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for {Email}", request.Email);
            return StatusCode(
                500,
                CreateProblemDetails("Internal Server Error", "An unexpected error occurred.", 500)
            );
        }
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <returns>Authentication response with JWT tokens</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(OperationResult<AuthResponse>), 201)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<ActionResult<OperationResult<AuthResponse>>> Register(
        [FromBody] RegisterRequest request
    )
    {
        try
        {
            var command = new RegisterCommand
            {
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IpAddress = GetClientIpAddress(),
                UserAgent = Request.Headers.UserAgent.ToString(),
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetProfile), null, result);
        }
        catch (DuplicateEmailException ex)
        {
            return BadRequest(CreateProblemDetails("Duplicate Email", ex.Message, 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error for {Email}", request.Email);
            return StatusCode(
                500,
                CreateProblemDetails("Internal Server Error", "An unexpected error occurred.", 500)
            );
        }
    }

    // /// <summary>
    // /// User logout
    // /// </summary>
    // /// <returns>Logout confirmation</returns>
    // [HttpPost("logout")]
    // [Authorize]
    // [ProducesResponseType(typeof(OperationResult<AuthResponse>), 200)]
    // [ProducesResponseType(typeof(ProblemDetails), 401)]
    // [ProducesResponseType(typeof(ProblemDetails), 500)]
    // public async Task<ActionResult<OperationResult<AuthResponse>>> Logout(
    //     [FromBody] RefreshTokenRequest? request = null
    // )
    // {
    //     try
    //     {
    //         var userId = GetCurrentUserId();
    //         var accessToken = GetAccessToken();
    //
    //         if (userId == Guid.Empty)
    //         {
    //             return BadRequest(CreateProblemDetails("Bad Request", "Invalid user ID.", 400));
    //         }
    //
    //         if (string.IsNullOrEmpty(accessToken))
    //         {
    //             return BadRequest(
    //                 CreateProblemDetails("Bad Request", "Access token is required.", 400)
    //             );
    //         }
    //
    //         var command = new LogoutCommand
    //         {
    //             AccessToken = accessToken,
    //             RefreshToken = request?.RefreshToken ?? string.Empty,
    //             IpAddress = GetClientIpAddress(),
    //         };
    //
    //         var result = await _mediator.Send(command);
    //
    //         if (result.Success)
    //         {
    //             return Ok(result);
    //         }
    //
    //         return BadRequest(CreateProblemDetails("Logout Failed", result.Message, 400));
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Logout error for user {UserId}", GetCurrentUserId());
    //         return StatusCode(
    //             500,
    //             CreateProblemDetails("Internal Server Error", "An unexpected error occurred.", 500)
    //         );
    //     }
    // }

    /// <summary>
    /// Get user profile
    /// </summary>
    /// <returns>Current user's profile information</returns>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(OperationResult<UserProfileDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult<OperationResult<UserProfileDto>>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var query = new GetUserProfileQuery { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get profile error");
            return StatusCode(
                500,
                CreateProblemDetails("Internal Server Error", "An unexpected error occurred.", 500)
            );
        }
    }

    /// <summary>
    /// Soft deletes a user account by marking it as deleted without removing it from the database.
    /// This operation will deactivate the user account and invalidate all associated refresh tokens.
    /// User can be identified by either UserId or Email address.
    /// </summary>
    /// <param name="request">The soft delete request containing either UserId or Email to identify the user</param>
    [HttpPost("SoftDeleteUser")]
    public async Task<IActionResult> SoftDeleteUser([FromBody] SoftDeleteUserRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(OperationResult.ErrorResult("Request cannot be null."));
            }

            if (
                (request.UserId == null || request.UserId == Guid.Empty)
                && string.IsNullOrWhiteSpace(request.Email)
            )
            {
                return BadRequest(
                    OperationResult.ErrorResult("Either UserId or Email must be provided.")
                );
            }

            var command = new SoftDeleteUserCommand
            {
                UserId = request.UserId ?? Guid.Empty,
                Email = request.Email,
            };
            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return Ok(result);
            }

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(OperationResult.ErrorResult(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                OperationResult.ErrorResult(
                    "An unexpected error occurred while processing the request.",
                    ex.Message
                )
            );
        }
    }

    // Helper Functions
    private string GetClientIpAddress()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            ipAddress = "127.0.0.1";
        return ipAddress;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private string GetAccessToken()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        return authHeader.StartsWith("Bearer ") ? authHeader[7..] : string.Empty;
    }

    private ProblemDetails CreateProblemDetails(string title, string detail, int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = Request.Path,
        };
    }
}
