using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Auth;
using Csdsa.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Csdsa.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsExpiredAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken == null)
            return true;
        return refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow;
    }
}
