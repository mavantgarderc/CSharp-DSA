using Csdsa.Domain.Models.Auth;

namespace Csdsa.Application.Interfaces;

public interface IBlacklistedTokenRepository : IGenericRepository<BlacklistedToken>
{
    Task<BlacklistedToken?> GetByTokenIdAsync(string tokenId);
}
