using Csdsa.Domain.Models.Common;
using Csdsa.Domain.Repository.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Csdsa.Domain.Context;

namespace Csdsa.Domain.Repository.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).Where(x => !x.IsDeleted).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync(x => !x.IsDeleted);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync(x => !x.IsDeleted);
            var items = await _dbSet
                .Where(x => !x.IsDeleted)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet.Where(x => !x.IsDeleted);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task<T> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            return entity!;
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(entity);
                return true;
            }
            return false;
        }
    }
}
