using System.Security.Claims;
using Csdsa.Domain.Models.UserEntities;

namespace Csdsa.Application.Auth;

public interface IJwtService : IDisposable
{
    string GenerateAccessToken(User user, IEnumerable<string> roles);
    RefreshToken GenerateRefreshToken(string ipAddress);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string? GetTokenIdFromToken(string token);
    bool IsTokenExpired(string token);
}
