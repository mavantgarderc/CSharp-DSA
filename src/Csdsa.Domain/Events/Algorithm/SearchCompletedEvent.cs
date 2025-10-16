using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Algorithm;

public class SearchCompletedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public string AlgorithmType { get; }
    public bool Found { get; }

    public SearchCompletedEvent(string algorithmType, bool found)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        AlgorithmType = algorithmType;
        Found = found;
    }
}
