using Csdsa.Infrastructure.Context;
using Csdsa.Domain.Models.Common.UserEntities.User;
using Csdsa.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Csdsa.Infrastructure.Common.Repository;

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
    }
}
