using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Algorithm;

public class SortCompletedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public string AlgorithmType { get; }

    public SortCompletedEvent(string algorithmType)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        AlgorithmType = algorithmType;
    }
}
