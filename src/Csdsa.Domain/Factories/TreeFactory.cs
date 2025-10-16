namespace Csdsa.Domain.Factories;

public class TreeFactory
{
    public Aggregates.TreeAggregate.BinaryTree<T> CreateBinaryTree<T>()
    {
        return new Aggregates.TreeAggregate.BinaryTree<T>();
    }
}
