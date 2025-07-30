using Csdsa.Domain.Models.Common;

namespace Csdsa.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();

    IGenericRepository<T> Repository<T>()
        where T : BaseEntity;

    IUserRepository Users { get; }
}
