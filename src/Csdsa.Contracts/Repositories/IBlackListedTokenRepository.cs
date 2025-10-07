namespace Csdsa.Contracts.Repositories;

public interface IBlacklistedTokenRepository : IGenericRepository<BlacklistedTokenDto>
{
    Task<BlacklistedTokenDto?> GetByTokenIdAsync(string tokenId);
}
