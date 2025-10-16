using Csdsa.Domain.Common;

namespace Csdsa.Domain.Aggregates.ListAggregate;

public class LinkedList<T> : IAggregateRoot
{
    public Entities.ListNode<T>? Head { get; private set; }
    public Entities.ListNode<T>? Tail { get; private set; }

    public LinkedList()
    {
        Head = null;
        Tail = null;
    }

    public void Add(T value)
    {
        // Add logic
        // Raise NodeAddedEvent
    }
}
