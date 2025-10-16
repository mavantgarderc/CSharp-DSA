using Csdsa.Domain.Common;
using Csdsa.Domain.Entities;
using Csdsa.Domain.Aggregates.GraphAggregate;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.PathFinding;

public class ShortestPathService
{
    public AlgorithmResponse<List<Node<T>>> Dijkstra<T>(Graph<T, double> graph, T start, T end)
    {
        // Dijkstra logic
        return new AlgorithmResponse<List<Node<T>>>(
            result: new List<Node<T>>(),
            complexity: new ComplexityAnalysis("O((V + E) log V)", "O(V)", "O((V + E) log V)", "O((V + E) log V)", "O((V + E) log V)", "Dijkstra implementation", 0, graph.Nodes.Count),
            algorithmName: AlgorithmType.Dijkstra.ToString());
    }
}
