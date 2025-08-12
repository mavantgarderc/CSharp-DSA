namespace Csdsa.Domain.Models.Auth;

public class AuditLog : BaseEntity
{
    public string Action { get; set; } = default!;
    public string PerformedBy { get; set; } = default!;
    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Details { get; set; }
}
