namespace Csdsa.Domain.Entities;

public class LockFreeListNode<T>
{
    public T Value { get; private set; }
    public LockFreeListNode<T>? Next { get; set; }

    public LockFreeListNode(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
