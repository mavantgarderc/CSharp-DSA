namespace Csdsa.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
