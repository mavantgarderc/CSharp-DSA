namespace Csdsa.Contracts.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IGenericRepository<T> Repository<T>()
        where T : BaseEntity;

    IUserRepository Users { get; }
}
