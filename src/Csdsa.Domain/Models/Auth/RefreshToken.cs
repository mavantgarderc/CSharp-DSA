using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Models.Entities;

namespace Csdsa.Domain.Models.Auth;

public class RefreshToken : BaseEntity
{
    [Required]
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;

    [Required]
    public string CreateByIp { get; set; } = string.Empty;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
