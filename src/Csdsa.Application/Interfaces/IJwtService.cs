using System.Threading.Tasks;
using Csdsa.Domain.Models.Auth;

namespace Csdsa.Application.Interfaces;

public interface IJwtService : IDisposable
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateAccessTokenAsync(User user, string ipAddress);
    Task<RefreshToken> GenerateRefreshTokenAsync(User user, string ipAddress);
    Task<string> GenerateRefreshTokenAsync(string ipAddress);
    Task<(bool IsValid, User? User)> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string accessToken);
    Task<string?> GetTokenIdAsync(string accessToken);
    Task<bool> IsTokenBlacklistedAsync(string accessToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task BlacklistTokenAsync(string accessToken, Guid userId, string reason);
}
