using System.ComponentModel.DataAnnotations;

namespace Csdsa.Api.DTOs.Auth;

public class SoftDeleteUserRequest
{
    public Guid? UserId { get; set; }
    public string? Email { get; set; } = string.Empty;
}
