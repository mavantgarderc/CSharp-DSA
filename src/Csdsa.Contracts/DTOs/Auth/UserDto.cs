namespace Csdsa.Contracts.Dtos.Auth;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    ICollection<UserRoleDto> UserRoles
);
