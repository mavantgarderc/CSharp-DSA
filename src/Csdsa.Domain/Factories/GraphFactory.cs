namespace Csdsa.Domain.Factories;

public class GraphFactory
{
    public Aggregates.GraphAggregate.Graph<TNode, TWeight> CreateGraph<TNode, TWeight>()
    {
        return new Aggregates.GraphAggregate.Graph<TNode, TWeight>();
    }
}
