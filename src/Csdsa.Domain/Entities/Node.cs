namespace Csdsa.Domain.Entities;

public class Node<T>
{
    public T Value { get; private set; }
    public Guid Id { get; private set; }

    public Node(T value)
    {
        Id = Guid.NewGuid();
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
