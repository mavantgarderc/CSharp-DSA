using Csdsa.Contracts.Dtos.Auth;

namespace Csdsa.Contracts.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenDto?> GetByTokenAsync(string token);
    Task RevokeAsync(string token);
    Task<bool> IsExpiredAsync(string token);
}
