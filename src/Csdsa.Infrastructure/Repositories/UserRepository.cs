using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Models.Common.UserEntities.User;
using Csdsa.Infrastructure.Common.Repository;
using Csdsa.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Csdsa.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
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
    }
}
