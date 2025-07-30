namespace Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email, DateTime CreatedAtUtc);
