using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models.Auth;

public class Permission : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    public string Resource { get; set; } = string.Empty;
    [Required]
    public string Action { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
