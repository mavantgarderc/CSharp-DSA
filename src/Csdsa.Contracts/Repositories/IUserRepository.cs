using Csdsa.Contracts.Dtos.Auth;

namespace Csdsa.Contracts.Repositories;

public interface IUserRepository
{
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetUsernameAsync(string username);
    Task<bool> IsEmailTakenAsync(string email);
    Task<bool> IsUsernameTakenAsync(string username);
    Task<bool> SoftDeleteAsync(string email);
    Task<UserDto?> GetUserByRefreshTokenAsync(string refreshToken);
    Task<UserDto> GetUserWithRolesAndPermissionsAsync(Guid id);
}
