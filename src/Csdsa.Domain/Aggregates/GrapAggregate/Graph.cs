using Csdsa.Domain.Common;
using Csdsa.Domain.Exceptions;

namespace Csdsa.Domain.Aggregates.GraphAggregate;

public class Graph<TNode, TWeight> : IAggregateRoot
{
    private readonly List<Entities.Node<TNode>> _nodes;
    private readonly List<Entities.Edge<TNode, TWeight>> _edges;

    public IReadOnlyList<Entities.Node<TNode>> Nodes => _nodes.AsReadOnly();
    public IReadOnlyList<Entities.Edge<TNode, TWeight>> Edges => _edges.AsReadOnly();

    public Graph()
    {
        _nodes = new List<Entities.Node<TNode>>();
        _edges = new List<Entities.Edge<TNode, TWeight>>();
    }

    public void AddNode(Entities.Node<TNode> node)
    {
        if (_nodes.Contains(node)) throw new DomainException("Node already exists.");
        _nodes.Add(node);
        // Raise NodeAddedEvent
    }

    public void AddEdge(Entities.Node<TNode> source, Entities.Node<TNode> target, TWeight weight)
    {
        if (!_nodes.Contains(source) || !_nodes.Contains(target)) throw new DomainException("Node not found.");
        var edge = new Entities.Edge<TNode, TWeight>(source, target, weight);
        _edges.Add(edge);
        // Raise EdgeAddedEvent
    }
}
