using Csdsa.Domain.Common;
using Csdsa.Domain.Entities;
using Csdsa.Domain.Aggregates.GraphAggregate;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.Searching;

public class GraphSearchService
{
    public AlgorithmResponse<List<Node<T>>> Bfs<T>(Graph<T, object> graph, T start)
    {
        // BFS logic
        return new AlgorithmResponse<List<Node<T>>>(
            result: new List<Node<T>>(),
            complexity: new ComplexityAnalysis("O(V + E)", "O(V)", "O(V + E)", "O(V + E)", "O(V + E)", "BFS implementation", 0, graph.Nodes.Count),
            algorithmName: AlgorithmType.Bfs.ToString());
    }

    public AlgorithmResponse<List<Node<T>>> Dfs<T>(Graph<T, object> graph, T start)
    {
        // DFS logic
        return new AlgorithmResponse<List<Node<T>>>(
            result: new List<Node<T>>(),
            complexity: new ComplexityAnalysis("O(V + E)", "O(V)", "O(V + E)", "O(V + E)", "O(V + E)", "DFS implementation", 0, graph.Nodes.Count),
            algorithmName: AlgorithmType.Dfs.ToString());
    }
}
