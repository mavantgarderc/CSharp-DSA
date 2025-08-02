using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }
}
