using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Graph;

public class EdgeAddedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public Guid SourceNodeId { get; }
    public Guid TargetNodeId { get; }

    public EdgeAddedEvent(Guid sourceNodeId, Guid targetNodeId)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        SourceNodeId = sourceNodeId;
        TargetNodeId = targetNodeId;
    }
}
