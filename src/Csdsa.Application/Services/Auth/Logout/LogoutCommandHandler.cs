using Csdsa.Application.Interfaces;
using Csdsa.Domain.Exceptions;
using Csdsa.Domain.Models;
using MediatR;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Csdsa.Application.Services.Auth.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, OperationResult<Unit>>
{
    private readonly IJwtService _jwtService;
    private readonly IBlacklistedTokenRepository _blacklistedTokenRepository;
    private readonly ILogger _logger = Log.ForContext<LogoutCommandHandler>();

    public LogoutCommandHandler(
        IJwtService jwtService,
        IBlacklistedTokenRepository blacklistedTokenRepository)
    {
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _blacklistedTokenRepository = blacklistedTokenRepository ?? throw new ArgumentNullException(nameof(blacklistedTokenRepository));
    }

    public async Task<OperationResult<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.Information("Logout attempt for access token from IP: {IpAddress}", request.IpAddress);

            // Validate refresh token
            var (isValid, user) = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (!isValid || user == null)
            {
                _logger.Warning("Invalid or revoked refresh token provided for logout.");
                return OperationResult<Unit>.ErrorResult("Invalid refresh token.");
            }

            // Revoke refresh token
            await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);

            // Blacklist access token
            await _jwtService.BlacklistTokenAsync(request.AccessToken, user.Id, "User logout");

            _logger.Information("Logout successful for user: {UserId}", user.Id);
            return OperationResult<Unit>.SuccessResult(Unit.Value, "Logout successful.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error during logout for user with refresh token.");
            return OperationResult<Unit>.ErrorResult("An error occurred during logout.");
        }
    }
}
