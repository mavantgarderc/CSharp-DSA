using Csdsa.Application.Services.Auth;

namespace Csdsa.Application.DTOs;

public class UserProfileDto : UserDto
{
}

public class UserProfileResponseDto : UserProfileDto
{
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class CreateUserProfileDto : UserProfileDto
{
}

public class UpdateUserProfileDto : UserProfileDto
{
}
