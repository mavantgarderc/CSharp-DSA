namespace Csdsa.Domain.Aggregates.GraphAggregate;

public class Edge<TNode, TWeight>
{
    public Entities.Node<TNode> Source { get; private set; }
    public Entities.Node<TNode> Target { get; private set; }
    public TWeight Weight { get; private set; }

    public Edge(Entities.Node<TNode> source, Entities.Node<TNode> target, TWeight weight)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Weight = weight;
    }
}
