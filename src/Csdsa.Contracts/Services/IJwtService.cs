using Csdsa.Contracts.Dtos.Auth;

namespace Csdsa.Contracts.Interfaces;

public interface IJwtService : IDisposable
{
    Task<string> GenerateAccessTokenAsync(UserDto user);
    Task<string> GenerateAccessTokenAsync(UserDto user, string ipAddress);
    Task<RefreshTokenDto> GenerateRefreshTokenAsync(UserDto user, string ipAddress);
    Task<string> GenerateRefreshTokenAsync(string ipAddress);
    Task<(bool IsValid, UserDto? User)> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string accessToken);
    Task<string?> GetTokenIdAsync(string accessToken);
    Task<bool> IsTokenBlacklistedAsync(string accessToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task BlacklistTokenAsync(string accessToken, Guid userId, string reason);
}
