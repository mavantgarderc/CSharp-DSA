using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models.Entities;

public class Role : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
