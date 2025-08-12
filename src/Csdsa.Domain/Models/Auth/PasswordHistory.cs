namespace Csdsa.Domain.Models.Auth;

public class PasswordHistory : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
}
