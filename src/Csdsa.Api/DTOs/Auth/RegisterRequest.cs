using System.ComponentModel.DataAnnotations;

namespace Csdsa.Api.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character."
    )]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MinLength(2)]
    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MinLength(2)]
    [MaxLength(50)]
    public string? LastName { get; set; }
}
