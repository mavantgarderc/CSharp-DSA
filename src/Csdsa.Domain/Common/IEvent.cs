namespace Csdsa.Domain.Common;

public interface IEvent
{
    Guid EventId { get; }
    DateTime Timestamp { get; }
}
