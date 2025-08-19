using Csdsa.Application.DTOs.Auth;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Csdsa.Application.Services.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, OperationResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        ILogger<LogoutCommandHandler> logger
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OperationResult<AuthResponse>> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken
    )
    {

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Processing logout request for user {UserId} from IP {IpAddress}", 
                request.UserId, request.IpAddress);

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User not found during logout: {UserId}", request.UserId);
                return OperationResult<AuthResponse>.ErrorResult("User not found");
            }

            var refreshTokenToInvalidate = user.RefreshTokens?
                .FirstOrDefault(rt => rt.Token == request.RefreshToken && 
                                     rt.IsActive && 
                                     !rt.IsRevoked);

            if (refreshTokenToInvalidate != null)
            {
                refreshTokenToInvalidate.RevokedAt = DateTime.UtcNow;
                refreshTokenToInvalidate.RevokedByIp = request.IpAddress;
                refreshTokenToInvalidate.RevokedReason = "Logout";

                _logger.LogInformation("Refresh token revoked for user {UserId}", request.UserId);
            }
            else
            {
                _logger.LogWarning("Refresh token not found or already revoked for user {UserId}", request.UserId);
            }

            if (!string.IsNullOrEmpty(request.AccessToken))
            {
                try
                {
                    var tokenClaims = await _jwtService.ValidateTokenAsync(request.AccessToken);
                    var jti = tokenClaims?.FindFirst("jti")?.Value;
                    if (!string.IsNullOrEmpty(jti))
                    {
                        // Add JTI to blacklist (implement ITokenBlacklistService if needed)
                        // await _tokenBlacklistService.BlacklistTokenAsync(jti, tokenExpiry);
                        _logger.LogInformation("Access token marked for blacklisting: {Jti}", jti);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to extract JTI from access token during logout");
                }
            }

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("User {UserId} successfully logged out from IP {IpAddress}", 
                request.UserId, request.IpAddress);

            var emptyAuthResponse = new AuthResponse
            {
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                AccessTokenExpiry = DateTime.MinValue,
                RefreshTokenExpiry = DateTime.MinValue,
                User = null!
            };

            return OperationResult<AuthResponse>.SuccessResult(emptyAuthResponse);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error occurred during logout for user {UserId}", request.UserId);
            return OperationResult<AuthResponse>.ErrorResult("An error occurred during logout");
        }
    }
}
