namespace Csdsa.Contracts.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUsernameAsync(string username);
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
    Task<bool> SoftDeleteAsync(string email);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
    Task<User> GetUserWithRolesAndPermissionsAsync(Guid id);
}
