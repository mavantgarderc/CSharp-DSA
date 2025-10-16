namespace Csdsa.Domain.Aggregates.ListAggregate;

public class ListNode<T>
{
    public T Value { get; private set; }
    public ListNode<T>? Next { get; set; }

    public ListNode(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
