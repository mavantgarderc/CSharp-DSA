namespace Csdsa.Domain.Aggregates.HeapAggregate;

public class HeapNode<T>
{
    public T Value { get; private set; }
    public int Priority { get; private set; }

    public HeapNode(T value, int priority)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Priority = priority;
    }
}
