namespace Csdsa.Contracts.Dtos.Auth;

public record RoleDto(
    Guid Id,
    string Name,
    ICollection<UserRoleDto> UserRoles
);
