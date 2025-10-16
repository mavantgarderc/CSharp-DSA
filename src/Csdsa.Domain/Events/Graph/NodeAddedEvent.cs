using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Graph;

public class NodeAddedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public Guid NodeId { get; }
    public object NodeValue { get; }

    public NodeAddedEvent(Guid nodeId, object nodeValue)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        NodeId = nodeId;
        NodeValue = nodeValue;
    }
}
