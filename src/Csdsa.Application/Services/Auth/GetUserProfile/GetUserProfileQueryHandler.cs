using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Application.Services.Auth.GetUserProfile;
using Csdsa.Domain.Models;
using MediatR;
using Serilog;

namespace Csdsa.Application.Handlers.Auth;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, OperationResult<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger _logger = Log.ForContext<GetUserProfileQueryHandler>();

    // Hard-coded configuration values since IAppConfig doesn't expose these settings
    private const int REFRESH_TOKEN_EXPIRY_DAYS = 7;
    private const int ACCESS_TOKEN_EXPIRY_MINUTES = 15;
    private const int MAX_FAILED_ATTEMPTS = 5;
    private const int LOCKOUT_DURATION_MINUTES = 30;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IEmailService emailService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<OperationResult<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Getting user profile for UserId: {UserId}", request.UserId);

            if (!string.IsNullOrEmpty(request.AccessToken))
            {
                try
                {
                    var claimsPrincipal = await _jwtService.ValidateTokenAsync(request.AccessToken);
                    if (claimsPrincipal == null || !claimsPrincipal.Identity?.IsAuthenticated == true)
                    {
                        _logger.Warning("Invalid access token provided for UserId: {UserId}", request.UserId);
                        return OperationResult<UserProfileDto>.ErrorResult("Invalid access token");
                    }

                    var userIdClaim = claimsPrincipal.FindFirst("sub") ?? claimsPrincipal.FindFirst("userId");
                    if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var tokenUserId))
                    {
                        _logger.Warning("Unable to extract user ID from token for UserId: {UserId}", request.UserId);
                        return OperationResult<UserProfileDto>.ErrorResult("Invalid token format");
                    }

                    if (tokenUserId != request.UserId)
                    {
                        _logger.Warning("Token UserId mismatch. Token UserId: {TokenUserId}, Requested UserId: {RequestedUserId}", 
                            tokenUserId, request.UserId);
                        return OperationResult<UserProfileDto>.ErrorResult("Token does not belong to the requested user");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Token validation failed for UserId: {UserId}", request.UserId);
                    return OperationResult<UserProfileDto>.ErrorResult("Invalid access token");
                }
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.Warning("User not found with Id: {UserId}", request.UserId);
                return OperationResult<UserProfileDto>.ErrorResult("User not found");
            }

            if (user.IsDeleted)
            {
                _logger.Warning("Attempt to get profile for deleted user: {UserId}", request.UserId);
                return OperationResult<UserProfileDto>.ErrorResult("User account is no longer active");
            }

            var userRoles = new List<string>();

            var userProfile = new UserProfileDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                IsEmailVerified = user.IsEmailVerified,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles
            };

            _logger.Information("Successfully retrieved user profile for UserId: {UserId}", request.UserId);
            return OperationResult<UserProfileDto>.SuccessResult(userProfile);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while getting user profile for UserId: {UserId}", request.UserId);
            return OperationResult<UserProfileDto>.ErrorResult("An error occurred while retrieving user profile");
        }
    }
}
