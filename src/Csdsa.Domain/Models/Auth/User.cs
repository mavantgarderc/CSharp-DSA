using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models.Auth;

public class User : BaseEntity
{
    public string UserName { get; set; } = default!;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // Authentication Fields
    public bool IsEmailVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpires { get; set; }

    // Account Lockout
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;

    // Password Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpires { get; set; }

    // Roles
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<BlacklistedToken> BlacklistedTokens { get; set; } = new List<BlacklistedToken>();

    public bool IsActive { get; set; } = true;
    public bool SoftDelete { get; set; } = false;
}
