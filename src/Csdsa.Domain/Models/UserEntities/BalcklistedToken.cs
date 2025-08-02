using System.ComponentModel.DataAnnotations;
using Csdsa.Domain.Models;

public class BlacklistedToken : BaseEntity
{
    [Required]
    public string TokenId {get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime BlackListedAt {get; set;} = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;
}

