namespace Csdsa.Contracts.Dtos.Auth;

public record UserRoleDto(
    Guid UserId,
    UserDto User,
    Guid RoleId,
    RoleDto Role,
    DateTime AssignedAt
);
