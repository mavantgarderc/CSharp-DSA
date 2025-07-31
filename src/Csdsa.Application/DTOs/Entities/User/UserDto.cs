namespace Csdsa.Application.DTOs.Entities.User;

public record UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}
