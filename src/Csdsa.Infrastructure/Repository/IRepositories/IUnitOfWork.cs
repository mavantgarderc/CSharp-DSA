using Csdsa.Domain.Models.Common;
using Csdsa.Domain.Repository.Implementation;

namespace Csdsa.Domain.Repository.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
