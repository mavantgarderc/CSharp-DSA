using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Tree;

public class TreeBalancedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public Guid TreeId { get; }

    public TreeBalancedEvent(Guid treeId)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        TreeId = treeId;
    }
}
