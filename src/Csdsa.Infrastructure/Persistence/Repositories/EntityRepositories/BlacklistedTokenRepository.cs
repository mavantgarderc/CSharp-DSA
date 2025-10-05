using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Auth;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class BlacklistedTokenRepository : GenericRepository<BlacklistedToken>, IBlacklistedTokenRepository
{
    public BlacklistedTokenRepository(AppDbContext context) : base(context) { }

    public async Task<BlacklistedToken?> GetByTokenIdAsync(string tokenId)
    {
        return await _context.BlacklistedTokens.FirstOrDefaultAsync(bt => bt.TokenId == tokenId);
    }
}
