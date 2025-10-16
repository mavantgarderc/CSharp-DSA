using Csdsa.Domain.Common;

namespace Csdsa.Domain.Aggregates.TreeAggregate;

public class BinaryTree<T> : IAggregateRoot
{
    public TreeNode<T>? Root { get; private set; }

    public BinaryTree()
    {
        Root = null;
    }

    public void Insert(T value)
    {
        // Insert logic
        // Raise NodeInsertedEvent
    }
}
