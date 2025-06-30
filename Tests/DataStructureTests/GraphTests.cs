using Modules.DataStructures;
using Xunit;

namespace Tests
{
    public class GraphTests
    {
        private Graph<int> MakeSimplePathGraph()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 3);
            g.AddEdge(2, 3, 4);
            return g;
        }

        private Graph<int> MakeDirectedCycleGraph()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 0, 1);
            return g;
        }

        private Graph<int> MakeDisconnectedGraph()
        {
            var g = new Graph<int>();
            g.AddVertex(0);
            g.AddVertex(1);
            g.AddVertex(2);
            g.AddVertex(3);
            return g;
        }

        [Fact]
        public void AddVertex_Idempotent()
        {
            var g = new Graph<int>();
            g.AddVertex(1);
            g.AddVertex(1);
            Assert.Empty(g.GetNeighbors(1));
        }

        [Fact]
        public void AddAndRemoveEdge()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2);
            Assert.True(g.IsConnected(1, 2));
            g.RemoveEdge(1, 2);
            Assert.False(g.IsConnected(1, 2));
        }

        [Fact]
        public void RemoveVertex_RemovesAllIncidentEdges()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2);
            g.AddEdge(2, 1);
            g.RemoveVertex(2);
            Assert.False(g.IsConnected(1, 2));
            Assert.False(g.IsConnected(2, 1));
        }

        [Fact]
        public void GetNeighbors_ReturnsEmptyForNonExistentVertex()
        {
            var g = new Graph<int>();
            Assert.Empty(g.GetNeighbors(12345));
        }

        [Fact]
        public void ReverseGraph_ReversesAllEdges()
        {
            var g = MakeSimplePathGraph();
            g.ReverseGraph();
            Assert.Empty(g.GetNeighbors(0));
            Assert.Equal(new[] { 0 }, g.GetNeighbors(1));
            Assert.Equal(new[] { 1 }, g.GetNeighbors(2));
            Assert.Equal(new[] { 2 }, g.GetNeighbors(3));
        }

        [Fact]
        public void ToDot_CorrectFormat()
        {
            var g = MakeSimplePathGraph();
            string dot = g.ToDot();
            Assert.Contains("digraph G", dot);
            Assert.Contains("0 -> 1", dot);
            Assert.Contains("1 -> 2", dot);
            Assert.Contains("2 -> 3", dot);
            Assert.Contains("}", dot);
        }

        [Fact]
        public void BFS_HandlesDisconnected()
        {
            var g = MakeDisconnectedGraph();
            var bfs = g.BFS(0);
            Assert.Single(bfs);
        }

        [Fact]
        public void BFS_HandlesCycles()
        {
            var g = MakeDirectedCycleGraph();
            var bfs = g.BFS(0);
            Assert.Equal(3, bfs.Count);
            Assert.All(new[] { 0, 1, 2 }, x => Assert.Contains(x, bfs));
        }

        [Fact]
        public void DFS_HandlesDisconnected()
        {
            var g = MakeDisconnectedGraph();
            var dfs = g.DFS(2);
            Assert.Single(dfs);
        }

        [Fact]
        public void DFS_Recursive_HandlesCycles()
        {
            var g = MakeDirectedCycleGraph();
            var dfs = g.DFS_Recursive(0);
            Assert.Equal(3, dfs.Count);
        }

        [Fact]
        public void TopologicalSort_ThrowsOnCycle()
        {
            var g = MakeDirectedCycleGraph();
            Assert.Throws<Exception>(() => g.TopologicalSort());
        }

        [Fact]
        public void TopologicalSort_RespectsOrder()
        {
            var g = MakeSimplePathGraph();
            var order = g.TopologicalSort();
            Assert.True(order.IndexOf(0) < order.IndexOf(1));
            Assert.True(order.IndexOf(1) < order.IndexOf(2));
            Assert.True(order.IndexOf(2) < order.IndexOf(3));
        }

        [Fact]
        public void FindAllPaths_FindsAllPaths()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 3);
            g.AddEdge(2, 3);
            var paths = g.FindAllPaths(0, 3);
            Assert.Equal(2, paths.Count);
            Assert.Contains(paths, p => p.SequenceEqual(new[] { 0, 1, 3 }));
            Assert.Contains(paths, p => p.SequenceEqual(new[] { 0, 2, 3 }));
        }

        [Fact]
        public void CountPaths_CorrectCount()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 2);
            g.AddEdge(2, 3);
            Assert.Equal(2, g.CountPaths(0, 3));
        }

        [Fact]
        public void ShortestPathBFS_FindsShortest()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(0, 2);
            var path = g.ShortestPathBFS(0, 2);
            Assert.Equal(new[] { 0, 2 }, path);
        }

        [Fact]
        public void Dijkstra_HandlesUnreachable()
        {
            var g = MakeDisconnectedGraph();
            var path = g.Dijkstra(0, 1);
            Assert.Empty(path);
        }

        [Fact]
        public void Dijkstra_FindsShortestWeightedPath()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 10);
            var path = g.Dijkstra(0, 2);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void HasCycle_TrueForCycle()
        {
            var g = MakeDirectedCycleGraph();
            Assert.True(g.HasCycle());
        }

        [Fact]
        public void HasCycle_FalseForAcyclic()
        {
            var g = MakeSimplePathGraph();
            Assert.False(g.HasCycle());
        }

        [Fact]
        public void HasCycle_UndirectedGraph()
        {
            var g = new Graph<int>(isDirected: false);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            Assert.True(g.HasCycle());
        }

        [Fact]
        public void FindCycle_ReturnsCycle()
        {
            var g = MakeDirectedCycleGraph();
            var cycle = g.FindCycle();
            Assert.True(cycle.Count > 0);
            Assert.True(g.IsConnected(cycle[^1], cycle[0]));
        }

        [Fact]
        public void IsGraphConnected_TrueForConnected()
        {
            var g = MakeSimplePathGraph();
            Assert.True(g.IsGraphConnected());
        }

        [Fact]
        public void IsGraphConnected_FalseForDisconnected()
        {
            var g = MakeDisconnectedGraph();
            Assert.False(g.IsGraphConnected());
        }

        [Fact]
        public void ConnectedComponents_FindsAll()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1);
            g.AddEdge(2, 3);
            var comps = g.ConnectedComponents();
            Assert.Equal(2, comps.Count);
            Assert.All(comps, c => Assert.True(c.Count >= 2));
        }

        [Fact]
        public void CountConnectedComponents_CorrectCount()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1);
            g.AddEdge(2, 3);
            g.AddVertex(4); // isolated vertex
            Assert.Equal(3, g.CountConnectedComponents());
        }

        [Fact]
        public void GraphColoring_AssignsColorsCorrectly()
        {
            var graph = new Graph<char>();
            graph.AddEdge('A', 'B');
            graph.AddEdge('B', 'C');

            var colors = graph.GraphColoring(3);

            Assert.Equal(0, colors['A']);
            Assert.NotEqual(colors['A'], colors['B']);
            Assert.NotEqual(colors['B'], colors['C']);
        }

        [Fact]
        public void GraphColoring_HandlesDisconnectedGraph()
        {
            var graph = new Graph<char>();
            graph.AddVertex('X');
            graph.AddVertex('Y');
            graph.AddVertex('Z');

            var colors = graph.GraphColoring(1);

            Assert.Equal(0, colors['X']);
            Assert.Equal(0, colors['Y']);
            Assert.Equal(0, colors['Z']);
        }

        [Fact]
        public void GraphColoring_ThrowsOnImpossible()
        {
            var graph = new Graph<char>();
            graph.AddEdge('A', 'B');
            graph.AddEdge('B', 'C');
            graph.AddEdge('C', 'A'); // triangle

            var ex = Record.Exception(() => graph.GraphColoring(2));
            Assert.NotNull(ex);
            Assert.Contains("Impossible Coloring", ex.Message);
        }

        [Fact]
        public void GraphColoring_AllowsMaxColorUse()
        {
            var graph = new Graph<int>();
            graph.AddEdge(1, 2);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);

            var colors = graph.GraphColoring(2);

            // Ensure no two adjacent nodes have same color
            foreach (var node in graph.Vertices)
            {
                foreach (var neighbor in graph.GetNeighbors(node))
                {
                    Assert.NotEqual(colors[node], colors[neighbor]);
                }
            }
        }

        [Fact]
        public void AStar_HandlesUnreachable()
        {
            var g = MakeDisconnectedGraph();
            var path = g.AStar(0, 1, (a, b) => 0);
            Assert.Empty(path);
        }

        [Fact]
        public void AStar_FindsOptimalPath()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 10);
            var path = g.AStar(0, 2, (a, b) => 0);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void BellmanFord_NegativeCycleThrows()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, -2);
            g.AddEdge(2, 0, -2);
            Assert.Throws<Exception>(() => g.BellmanFord(0, 2));
        }

        [Fact]
        public void BellmanFord_FindsPath()
        {
            var g = new Graph<int>();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            var path = g.BellmanFord(0, 2);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void KruskalMST_IncludesAllNodes()
        {
            var g = new Graph<int>(isDirected: false);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 3);
            g.AddEdge(0, 3, 4);
            var mst = g.KruskalMST();
            var vertices = mst.SelectMany(e => new[] { e.src, e.dst }).Distinct().ToList();
            Assert.True(vertices.Count >= 4);
            Assert.Equal(3, mst.Count);
        }

        [Fact]
        public void PrimMST_HandlesEmptyGraph()
        {
            var g = new Graph<int>();
            var mst = g.PrimMST();
            Assert.Empty(mst);
        }

        [Fact]
        public void PrimMST_BuildsSpanningTree()
        {
            var g = new Graph<int>(isDirected: false);
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 3);
            g.AddEdge(0, 3, 4);
            var mst = g.PrimMST();
            var vertices = mst.SelectMany(e => new[] { e.src, e.dst }).Distinct().ToList();
            Assert.True(vertices.Count >= 4);
            Assert.Equal(3, mst.Count);
        }

        [Fact]
        public void RemoveVertex_NonexistentVertexDoesNotThrow()
        {
            var g = new Graph<int>();
            g.AddVertex(1);
            g.RemoveVertex(999);
            Assert.True(g.GetNeighbors(1) is List<int>);
        }

        [Fact]
        public void RemoveEdge_NonexistentEdgeDoesNotThrow()
        {
            var g = new Graph<int>();
            g.AddVertex(1);
            g.AddVertex(2);
            g.RemoveEdge(1, 2);
            Assert.True(g.GetNeighbors(1) is List<int>);
        }

        [Fact]
        public void VertexCount_ReturnsCorrectCount()
        {
            var g = new Graph<int>();
            Assert.Equal(0, g.VertexCount);
            g.AddVertex(1);
            g.AddVertex(2);
            Assert.Equal(2, g.VertexCount);
        }

        [Fact]
        public void EdgeCount_ReturnsCorrectCount()
        {
            var g = new Graph<int>();
            Assert.Equal(0, g.EdgeCount);
            g.AddEdge(1, 2);
            Assert.Equal(1, g.EdgeCount);
        }

        [Fact]
        public void EdgeCount_UndirectedGraph()
        {
            var g = new Graph<int>(isDirected: false);
            g.AddEdge(1, 2);
            Assert.Equal(1, g.EdgeCount); // Should count undirected edge once
        }

        [Fact]
        public void GetNeighborsWithWeights_ReturnsWeights()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2, 5);
            g.AddEdge(1, 3, 10);
            var neighbors = g.GetNeighborsWithWeights(1);
            Assert.Equal(2, neighbors.Count);
            Assert.Contains((2, 5), neighbors);
            Assert.Contains((3, 10), neighbors);
        }

        [Fact]
        public void HasEdge_DetectsEdgeExistence()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2);
            Assert.True(g.HasEdge(1, 2));
            Assert.False(g.HasEdge(2, 1)); // directed graph
        }

        [Fact]
        public void ToJson_ProducesValidJson()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2, 3);
            var json = g.ToJson();
            Assert.Contains("IsDirected", json);
            Assert.Contains("Vertices", json);
            Assert.Contains("Edges", json);
        }

        [Fact]
        public void Clone_CreatesIdenticalGraph()
        {
            var g = MakeSimplePathGraph();
            var clone = g.Clone();

            Assert.Equal(g.VertexCount, clone.VertexCount);
            Assert.Equal(g.EdgeCount, clone.EdgeCount);
            Assert.Equal(g.IsDirected, clone.IsDirected);

            foreach (var vertex in g.Vertices)
            {
                Assert.Equal(g.GetNeighbors(vertex), clone.GetNeighbors(vertex));
            }
        }

        [Fact]
        public void Adj_Property_ExposesAdjacencyList()
        {
            var g = new Graph<int>();
            g.AddEdge(1, 2);
            g.AddEdge(1, 3);

            var adj = g.Adj;
            Assert.True(adj.ContainsKey(1));
            Assert.Equal(2, adj[1].Count);
            Assert.Contains(2, adj[1]);
            Assert.Contains(3, adj[1]);
        }
    }
}
