namespace Csdsa.Domain.Events;

public record UserRegisteredEvent(Guid UserId, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
