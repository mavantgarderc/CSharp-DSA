namespace Csdsa.Domain.Entities;

public class ConcurrentNode<T>
{
    public T Value { get; private set; }
    public Guid Id { get; private set; }

    public ConcurrentNode(T value)
    {
        Id = Guid.NewGuid();
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
