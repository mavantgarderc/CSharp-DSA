using System.ComponentModel.DataAnnotations;

namespace Csdsa.Domain.Models.Auth;

public class RefreshToken : BaseEntity
{
    [Required]
    [StringLength(512, MinimumLength = 1)]
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    [Required]
    [StringLength(45)]
    public string CreatedByIp { get; set; } = string.Empty;

    public DateTime? RevokedAt { get; set; }

    [StringLength(45)]
    public string? RevokedByIp { get; set; }

    public string? RevokedReason { get; set; }

    [StringLength(512)]
    public string? ReplacedByToken { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke(string revokedByIp, string? replacedByToken = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token is already revoked");

        if (string.IsNullOrWhiteSpace(revokedByIp))
            throw new ArgumentException(
                "Revoked by IP cannot be null or empty",
                nameof(revokedByIp)
            );

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedByIp = revokedByIp;
        ReplacedByToken = replacedByToken;
    }

    public bool IsValidForRefresh()
    {
        return IsActive && !string.IsNullOrWhiteSpace(Token);
    }

    public TimeSpan GetRemainingTime()
    {
        var remaining = ExpiresAt - DateTime.UtcNow;
        return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
    }

    public override string ToString()
    {
        var tokenPreview = string.IsNullOrEmpty(Token)
            ? "null"
            : $"{Token[..Math.Min(8, Token.Length)]}...";
        return $"RefreshToken[Id={Id}, TokenPreview={tokenPreview}, UserId={UserId}, "
            + $"ExpiresAt={ExpiresAt:yyyy-MM-dd HH:mm:ss}, IsActive={IsActive}]";
    }
}
