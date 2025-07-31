using Csdsa.Domain.Enums;

namespace Csdsa.Application.DTOs.Entities.Role;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UserRole RoleType { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
