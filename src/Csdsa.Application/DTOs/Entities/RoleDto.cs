using Csdsa.Domain.Enums;

namespace Csdsa.Application.DTOs.Entities.Role;

public record RoleDto(
    int Id,
    string Name,
    string Description,
    UserRole RoleType,
    int UserCount,
    DateTime CreatedAt
);
