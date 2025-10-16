namespace Csdsa.Domain.Entities;

public class Edge<TNode, TWeight>
{
    public Node<TNode> Source { get; private set; }
    public Node<TNode> Target { get; private set; }
    public TWeight Weight { get; private set; }

    public Edge(Node<TNode> source, Node<TNode> target, TWeight weight)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Weight = weight;
    }
}
