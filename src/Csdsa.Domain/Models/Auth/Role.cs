using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models.Auth;

public class Role : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
