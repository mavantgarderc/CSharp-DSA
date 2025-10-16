namespace Csdsa.Domain.Entities;

public class ListNode<T>
{
    public T Value { get; private set; }
    public ListNode<T>? Next { get; set; }
    public ListNode<T>? Previous { get; set; }

    public ListNode(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
