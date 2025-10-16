using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Algorithm;

public class PathFoundEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public string AlgorithmType { get; }
    public List<Guid> Path { get; }

    public PathFoundEvent(string algorithmType, List<Guid> path)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        AlgorithmType = algorithmType;
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }
}
