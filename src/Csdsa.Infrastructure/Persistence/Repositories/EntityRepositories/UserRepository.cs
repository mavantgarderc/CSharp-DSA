using System.Linq.Expressions;
using Csdsa.Application.Interfaces;
using Csdsa.Domain.Models.Auth;
using Csdsa.Infrastructure.Persistence.Context;
using Csdsa.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Csdsa.Infrastructure.Repositories.EntityRepositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context)
        : base(context) { }

    public override Task<User> AddAsync(User entity)
    {
        return base.AddAsync(entity);
    }

    public override Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities)
    {
        return base.AddRangeAsync(entities);
    }

    public override Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
    {
        return base.AnyAsync(predicate);
    }

    public override Task<int> CountAsync()
    {
        return base.CountAsync();
    }

    public override Task<int> CountAsync(Expression<Func<User, bool>> predicate)
    {
        return base.CountAsync(predicate);
    }

    public override Task<User> DeleteAsync(Guid id)
    {
        return base.DeleteAsync(id);
    }

    public override Task<User> DeleteAsync(User entity)
    {
        return base.DeleteAsync(entity);
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public override Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return base.FindAsync(predicate);
    }

    public override Task<IEnumerable<User>> FindAsync(
        Expression<Func<User, bool>> predicate,
        params Expression<Func<User, object>>[] includes
    )
    {
        return base.FindAsync(predicate, includes);
    }

    public override Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        return base.FirstOrDefaultAsync(predicate);
    }

    public override Task<IEnumerable<User>> GetAllAsync()
    {
        return base.GetAllAsync();
    }

    public override Task<IEnumerable<User>> GetAllAsync(
        Expression<Func<User, bool>>? filter = null,
        Func<IQueryable<User>, IOrderedQueryable<User>>? orderBy = null,
        params Expression<Func<User, object>>[] includes
    )
    {
        return base.GetAllAsync(filter, orderBy, includes);
    }

    public override Task<IEnumerable<User>> GetAllAsync(
        params Expression<Func<User, object>>[] includes
    )
    {
        return base.GetAllAsync(includes);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
    }

    public override Task<User?> GetByIdAsync(Guid id)
    {
        return base.GetByIdAsync(id);
    }

    public override Task<User?> GetByIdAsync(
        Guid id,
        params Expression<Func<User, object>>[] includes
    )
    {
        return base.GetByIdAsync(id, includes);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize
    )
    {
        return base.GetPagedAsync(page, pageSize);
    }

    public override Task<(IEnumerable<User> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<User, bool>>? filter = null
    )
    {
        return base.GetPagedAsync(page, pageSize, filter);
    }

    public async Task<User?> GetUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserName == username && !u.IsDeleted);
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<bool> IsUsernameTakenAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.UserName == username && !u.IsDeleted);
    }

    public virtual async Task<bool> SoftDeleteAsync(string email)
    {
        var entity = await GetByEmailAsync(email);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
            return true;
        }
        return false;
    }

    public override Task<bool> SoftDeleteAsync(Guid id)
    {
        return base.SoftDeleteAsync(id);
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    public override Task<User> UpdateAsync(User entity)
    {
        return base.UpdateAsync(entity);
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return await FirstOrDefaultAsync(
            u => u.RefreshTokens.Any(rt => rt.Token == refreshToken),
            u => u.RefreshTokens,
            u => u.Role
        );
    }
}
