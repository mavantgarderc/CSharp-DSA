using System.Security.Claims;
using Csdsa.Domain.Models.Auth;

namespace Csdsa.Application.Interfaces;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    Task<string?> GetTokenIdAsync(string token);
    Task<bool> IsTokenBlacklistedAsync(string tokenId);
    Task BlacklistTokenAsync(string tokenId, Guid userId, string reason);
}
