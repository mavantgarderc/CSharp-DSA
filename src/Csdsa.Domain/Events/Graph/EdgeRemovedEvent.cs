using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Graph;

public class EdgeRemovedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public Guid SourceNodeId { get; }
    public Guid TargetNodeId { get; }

    public EdgeRemovedEvent(Guid sourceNodeId, Guid targetNodeId)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        SourceNodeId = sourceNodeId;
        TargetNodeId = targetNodeId;
    }
}
