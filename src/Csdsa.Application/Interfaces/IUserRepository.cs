using Csdsa.Application.Common.Interfaces;
using Csdsa.Domain.Models.Common.UserEntities.User;

namespace Csdsa.Application.Common.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUsernameAsync(string username);
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
}
