namespace Csdsa.Contracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IUserRepository Users { get; }
    IBlacklistedTokenRepository IBlacklistedTokens { get; }
    IPasswordHasher IPasswordHashers { get; }
    IRefreshTokenRepository IRefreshTokens { get; }
}
