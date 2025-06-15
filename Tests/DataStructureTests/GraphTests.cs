using System;
using System.Collections.Generic;
using System.Linq;
using Modules.DataStructures;
using Xunit;

namespace Modules.DataStructures.Tests
{
    public class GraphTests
    {
        // Helpers
        private Graph MakeSimplePathGraph()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 2);
            g.AddEdge(1, 2, 3);
            g.AddEdge(2, 3, 4);
            return g;
        }

        private Graph MakeDirectedCycleGraph()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(2, 0, 1);
            return g;
        }

        private Graph MakeDisconnectedGraph()
        {
            var g = new Graph();
            g.AddNode(0);
            g.AddNode(1);
            g.AddNode(2);
            g.AddNode(3);
            return g;
        }

        [Fact]
        public void AddNode_Idempotent()
        {
            var g = new Graph();
            g.AddNode(1);
            g.AddNode(1);
            // The node exists, but has no neighbors yet
            Assert.Empty(g.GetNeighbors(1));
        }

        [Fact]
        public void AddAndRemoveEdge()
        {
            var g = new Graph();
            g.AddEdge(1, 2);
            Assert.True(g.IsConnected(1, 2));
            g.RemoveEdge(1, 2);
            Assert.False(g.IsConnected(1, 2));
        }

        [Fact]
        public void RemoveNode_RemovesAllIncidentEdges()
        {
            var g = new Graph();
            g.AddEdge(1, 2);
            g.AddEdge(2, 1);
            g.RemoveNode(2);
            Assert.False(g.IsConnected(1, 2));
            Assert.False(g.IsConnected(2, 1));
        }

        [Fact]
        public void GetNeighbors_ReturnsEmptyForNonExistentNode()
        {
            var g = new Graph();
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
            Assert.Contains("0 => 1", dot);
            Assert.Contains("1 => 2", dot);
            Assert.Contains("2 => 3", dot);
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
            var g = new Graph();
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
            var g = new Graph();
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 2);
            g.AddEdge(2, 3);
            Assert.Equal(2, g.CountPaths(0, 3));
        }

        [Fact]
        public void ShortestPathBFS_FindsShortest()
        {
            var g = new Graph();
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
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 10);
            var path = g.Dijkstra(0, 2);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void HasCycleDirected_TrueForCycle()
        {
            var g = MakeDirectedCycleGraph();
            Assert.True(g.HasCycleDirected());
        }

        [Fact]
        public void HasCycleDirected_FalseForAcyclic()
        {
            var g = MakeSimplePathGraph();
            Assert.False(g.HasCycleDirected());
        }

        [Fact]
        public void FindCycle_ReturnsCycle()
        {
            var g = MakeDirectedCycleGraph();
            var cycle = g.FindCycle();
            Assert.True(cycle.Count > 0);
            // The returned path should be a cycle: last connects to first
            Assert.True(g.IsConnected(cycle[^1], cycle[0]));
        }

        [Fact]
        public void IsConnected_TrueForConnected()
        {
            var g = MakeSimplePathGraph();
            Assert.True(g.IsConnected());
        }

        [Fact]
        public void IsConnected_FalseForDisconnected()
        {
            var g = MakeDisconnectedGraph();
            Assert.False(g.IsConnected());
        }

        [Fact]
        public void COnnectedComponents_FindsAll()
        {
            var g = new Graph();
            g.AddEdge(0, 1);
            g.AddEdge(2, 3);
            var comps = g.COnnectedComponents();
            Assert.Equal(2, comps.Count);
            Assert.All(comps, c => Assert.True(c.Count >= 2));
        }

        [Fact]
        public void GraphColoring_ThrowsOnImpossible()
        {
            // For this test, coloring is "directed", so triangle with all edges one-way doesn't break
            // Make undirected triangle by adding both directions
            var g = new Graph();
            g.AddEdge(0, 1);
            g.AddEdge(1, 0);
            g.AddEdge(1, 2);
            g.AddEdge(2, 1);
            g.AddEdge(2, 0);
            g.AddEdge(0, 2);
            Assert.Throws<Exception>(() => g.GraphColoring(2));
        }

        [Fact]
        public void GraphColoring_AssignsColors()
        {
            var g = MakeSimplePathGraph();
            var colors = g.GraphColoring(2);
            // For each node, all outgoing neighbors must have different color (matches directed logic)
            for (int i = 0; i < colors.Length; ++i)
            {
                foreach (var n in g.GetNeighbors(i))
                    Assert.NotEqual(colors[i], colors[n]);
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
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            g.AddEdge(0, 2, 10);
            var path = g.AStar(0, 2, (a, b) => 0);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void BellmanFord_NegativeCycleThrows()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, -2);
            g.AddEdge(2, 0, -2);
            Assert.Throws<Exception>(() => g.BellmanFord(0, 2));
        }

        [Fact]
        public void BellmanFord_FindsPath()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 1);
            var path = g.BellmanFord(0, 2);
            Assert.Equal(new[] { 0, 1, 2 }, path);
        }

        [Fact]
        public void KruskalMST_IncludesAllNodes()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 3);
            g.AddEdge(0, 3, 4);
            var mst = g.KruskalMST();
            var flat = mst.SelectMany(e => new[] { e.Item1, e.Item2 }).Distinct().ToList();
            Assert.True(flat.Count >= 4);
            Assert.Equal(3, mst.Count);
        }

        [Fact]
        public void PrimMST_HandlesEmptyGraph()
        {
            var g = new Graph();
            var mst = g.PrimMST();
            Assert.Empty(mst);
        }

        [Fact]
        public void PrimMST_BuildsSpanningTree()
        {
            var g = new Graph();
            g.AddEdge(0, 1, 1);
            g.AddEdge(1, 2, 2);
            g.AddEdge(2, 3, 3);
            g.AddEdge(0, 3, 4);
            var mst = g.PrimMST();
            var flat = mst.SelectMany(e => new[] { e.Item1, e.Item2 }).Distinct().ToList();
            Assert.True(flat.Count >= 4);
            Assert.Equal(3, mst.Count);
        }

        [Fact]
        public void RemoveNode_NonexistentNodeDoesNotThrow()
        {
            var g = new Graph();
            g.AddNode(1);
            g.RemoveNode(999);
            Assert.True(g.GetNeighbors(1) is List<int>);
        }

        [Fact]
        public void RemoveEdge_NonexistentEdgeDoesNotThrow()
        {
            var g = new Graph();
            g.AddNode(1);
            g.AddNode(2);
            g.RemoveEdge(1, 2);
            Assert.True(g.GetNeighbors(1) is List<int>);
        }
    }
}
