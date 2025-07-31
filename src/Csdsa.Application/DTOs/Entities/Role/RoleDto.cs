using Csdsa.Domain.Models.Enums;

namespace Csdsa.Application.DTOs.Entities.Role;

public record RoleDto
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UserRole RoleType { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
