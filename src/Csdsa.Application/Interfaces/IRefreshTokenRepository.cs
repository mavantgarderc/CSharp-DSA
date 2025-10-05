using Csdsa.Domain.Models.Auth;

namespace Csdsa.Application.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(string token);
    Task<bool> IsExpiredAsync(string token);
}
