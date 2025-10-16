using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Tree;

public class NodeInsertedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public object NodeValue { get; }

    public NodeInsertedEvent(object nodeValue)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        NodeValue = nodeValue;
    }
}
