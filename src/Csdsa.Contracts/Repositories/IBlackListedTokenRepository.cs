using Csdsa.Contracts.Dtos.Auth;

namespace Csdsa.Contracts.Repositories;

public interface IBlacklistedTokenRepository
{
    Task<BlacklistedTokenDto?> GetByTokenIdAsync(string tokenId);
}
