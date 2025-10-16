using Csdsa.Domain.Common;

namespace Csdsa.Domain.Events.Common;

public class ConflictDetectedEvent : IEvent
{
    public Guid EventId { get; }
    public DateTime Timestamp { get; }
    public string StructureType { get; }
    public string Operation { get; }

    public ConflictDetectedEvent(string structureType, string operation)
    {
        EventId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        StructureType = structureType;
        Operation = operation;
    }
}
