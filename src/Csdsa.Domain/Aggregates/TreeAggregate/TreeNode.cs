namespace Csdsa.Domain.Aggregates.TreeAggregate;

public class TreeNode<T>
{
    public T Value { get; private set; }
    public TreeNode<T>? Left { get; set; }
    public TreeNode<T>? Right { get; set; }

    public TreeNode(T value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }
}
