using Csdsa.Domain.Common;

namespace Csdsa.Domain.Aggregates.HeapAggregate;

public class Heap<T> : IAggregateRoot
{
    private readonly List<Entities.HeapNode<T>> _nodes;

    public IReadOnlyList<Entities.HeapNode<T>> Nodes => _nodes.AsReadOnly();

    public Heap()
    {
        _nodes = new List<Entities.HeapNode<T>>();
    }

    public void Insert(T value, int priority)
    {
        // Insert logic
        // Raise HeapReorderedEvent
    }
}
