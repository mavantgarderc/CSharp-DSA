using System.Security.Claims;
using Csdsa.Contracts.Repositories;
using Csdsa.Domain.Models;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Csdsa.Application.Services.Auth.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, OperationResult<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger = Log.ForContext<GetUserProfileQueryHandler>();

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<OperationResult<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                _logger.Warning("Unauthorized attempt to access user profile.");
                return OperationResult<UserProfileDto>.ErrorResult("User is not authenticated.");
            }

            var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContext.User.FindFirst("sub")?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
            {
                _logger.Warning("Invalid user ID in token claims.");
                return OperationResult<UserProfileDto>.ErrorResult("Invalid user ID.");
            }

            var user = await _userRepository.GetUserWithRolesAndPermissionsAsync(userId);
            if (user == null)
            {
                _logger.Warning("User not found for ID: {UserId}", userId);
                return OperationResult<UserProfileDto>.ErrorResult("User not found.");
            }

            var userProfile = new UserProfileDto
            {
                UserId = user.Id,
                Email = user.Email,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive,
                Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            };

            _logger.Information("Successfully retrieved profile for user: {UserId}", user.Id);
            return OperationResult<UserProfileDto>.SuccessResult(userProfile, "Profile retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving user profile for request.");
            return OperationResult<UserProfileDto>.ErrorResult("An error occurred while retrieving the user profile.");
        }
    }
}
