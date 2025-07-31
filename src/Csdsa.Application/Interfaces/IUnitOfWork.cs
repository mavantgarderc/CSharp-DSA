using Csdsa.Domain.Models;

namespace Csdsa.Application.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IGenericRepository<T> Repository<T>()
        where T : BaseEntity;

    IUserRepository Users { get; }
}
