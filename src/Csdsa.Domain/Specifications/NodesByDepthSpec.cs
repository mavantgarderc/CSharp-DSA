namespace Csdsa.Domain.Specifications;

public class NodesByDepthSpec<T> : Specification<Aggregates.TreeAggregate.TreeNode<T>>
{
    private readonly int _depth;

    public NodesByDepthSpec(int depth)
    {
        if (depth < 0) throw new ArgumentException("Depth cannot be negative.", nameof(depth));
        _depth = depth;
    }

    public override bool IsSatisfiedBy(Aggregates.TreeAggregate.TreeNode<T> entity)
    {
        // Depth check logic
        return true;
    }
}
