using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Modules.DataStructures;
using Modules.Algorithms;

namespace Modules.Tests.Algorithms
{
    public class BFSTests
    {
        private Graph<int> CreateSimpleGraph()
        {
            // 0 -- 1 -- 2
            // |         |
            // +----3----+
            var g = new Graph<int>(false); // undirected
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(0, 3);
            g.AddEdge(3, 2);
            return g;
        }

        [Fact]
        public void DistanceFromSource_Works()
        {
            var g = CreateSimpleGraph();
            var dist = BFS.DistanceFromSource(g, 0);
            // 0:0, 1:1, 2:2, 3:1
            Assert.Equal(0, dist[0]);
            Assert.Equal(1, dist[1]);
            Assert.Equal(2, dist[2]);
            Assert.Equal(1, dist[3]);
        }

        [Fact]
        public void DetectCycleUndirected_ReturnsFalseForAcyclic()
        {
            var g = new Graph<int>(false);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            Assert.False(BFS.DetectCycleUndirected(g));
        }

        [Fact]
        public void DetectCycleUndirected_ReturnsTrueForCycle()
        {
            var g = new Graph<int>(false);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            Assert.True(BFS.DetectCycleUndirected(g));
        }

        [Fact]
        public void TopologicalSortKahn_WorksOnDAG()
        {
            var g = new Graph<int>(true); // directed
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 3);
            g.AddEdge(2, 3);
            var topo = BFS.TopologicalSortKahn(g);
            Assert.Equal(4, topo.Count);
            Assert.True(topo.IndexOf(0) < topo.IndexOf(1));
            Assert.True(topo.IndexOf(0) < topo.IndexOf(2));
            Assert.True(topo.IndexOf(1) < topo.IndexOf(3));
            Assert.True(topo.IndexOf(2) < topo.IndexOf(3));
        }

        [Fact]
        public void WordLadder_Works()
        {
            var wordList = new List<string> { "hot", "dot", "dog", "lot", "log", "cog" };
            int steps = BFS.WordLadder("hit", "cog", wordList);
            Assert.Equal(5, steps); // hit->hot->dot->dog->cog or hit->hot->lot->log->cog
            Assert.Equal(0, BFS.WordLadder("hit", "zzz", wordList));
        }

        [Fact]
        public void MultiSourceBFS_Works()
        {
            var g = CreateSimpleGraph();
            var sources = new List<int> { 0, 2 };
            var dist = BFS.MultiSourceBFS(g, sources);
            Assert.Equal(0, dist[0]);
            Assert.Equal(1, dist[1]);
            Assert.Equal(0, dist[2]);
            Assert.Equal(1, dist[3]);
        }

        [Fact]
        public void ConnectedComponents_Works()
        {
            var g = new Graph<int>(false);
            g.AddEdge(0, 1);
            g.AddEdge(2, 3);
            var comps = BFS.ConnectedComponents(g);
            Assert.Equal(2, comps.Count);
            Assert.Contains(comps, c => c.Contains(0) && c.Contains(1));
            Assert.Contains(comps, c => c.Contains(2) && c.Contains(3));
        }

        [Fact]
        public void AllPathsBFS_Works()
        {
            // 0 -> 1 -> 3
            //   \> 2 -/
            var g = new Graph<int>(true); // directed
            g.AddEdge(0, 1);
            g.AddEdge(1, 3);
            g.AddEdge(0, 2);
            g.AddEdge(2, 3);
            var paths = BFS.AllPathsBFS(g, 0, 3, 4);
            var expected = new List<List<int>> { new() { 0, 1, 3 }, new() { 0, 2, 3 } };
            Assert.Equal(2, paths.Count);
            Assert.All(expected, p => Assert.Contains(paths, x => x.SequenceEqual(p)));
        }

        [Fact]
        public void KnightsPath_Works()
        {
            var start = new BFS.Point(0, 0);
            var end = new BFS.Point(1, 2);
            int moves = BFS.KnightsPath(8, start, end);
            Assert.Equal(1, moves);

            moves = BFS.KnightsPath(3, new BFS.Point(0, 0), new BFS.Point(2, 2));
            Assert.Equal(4, moves);
        }

        [Fact]
        public void BFSWithPredicate_Works()
        {
            var g = CreateSimpleGraph();
            int dist = BFS.BFSWithPredicate(g, 0, u => u == 2);
            Assert.Equal(2, dist);
            Assert.Equal(-1, BFS.BFSWithPredicate(g, 0, u => u == 99));
        }

        [Fact]
        public void NumberOfIslands_Works()
        {
            int[,] grid = { { 1, 1, 0, 0 }, { 1, 0, 0, 1 }, { 0, 0, 1, 1 }, { 0, 0, 0, 0 } };
            int count = BFS.NumberOfIslands(grid);
            Assert.Equal(2, count);
        }

        [Fact]
        public void ShortestPathInGrid_Works()
        {
            int[,] grid = { { 0, 0, 1 }, { 1, 0, 1 }, { 1, 0, 0 } };
            var start = new BFS.Point(0, 0);
            var end = new BFS.Point(2, 2);
            int d = BFS.ShortestPathInGrid(grid, start, end);
            Assert.Equal(4, d);

            int[,] blocked = { { 0, 1 }, { 1, 0 } };
            Assert.Equal(-1, BFS.ShortestPathInGrid(blocked, new BFS.Point(0, 0), new BFS.Point(1, 1)));
        }

        // Additional tests for the generic Graph<T> BFS methods
        [Fact]
        public void GraphBFS_Works()
        {
            var g = CreateSimpleGraph();
            var traversal = g.BFS(0);
            Assert.Equal(4, traversal.Count);
            Assert.Equal(0, traversal[0]); // starts with 0
            Assert.Contains(1, traversal);
            Assert.Contains(2, traversal);
            Assert.Contains(3, traversal);
        }

        [Fact]
        public void ShortestPathBFS_Works()
        {
            var g = CreateSimpleGraph();
            var path = g.ShortestPathBFS(0, 2);
            Assert.Equal(3, path.Count);
            Assert.Equal(0, path[0]);
            Assert.Equal(2, path[2]);
            // Path should be either [0,1,2] or [0,3,2]
            Assert.True(path.SequenceEqual(new[] { 0, 1, 2 }) || path.SequenceEqual(new[] { 0, 3, 2 }));
        }

        [Fact]
        public void ConnectedComponents_GraphMethod_Works()
        {
            var g = new Graph<int>(false);
            g.AddEdge(0, 1);
            g.AddEdge(2, 3);
            var components = g.ConnectedComponents();
            Assert.Equal(2, components.Count);
            Assert.Contains(components, c => c.Contains(0) && c.Contains(1));
            Assert.Contains(components, c => c.Contains(2) && c.Contains(3));
        }

        [Fact]
        public void IsGraphConnected_Works()
        {
            var connectedGraph = CreateSimpleGraph();
            Assert.True(connectedGraph.IsGraphConnected());

            var disconnectedGraph = new Graph<int>(false);
            disconnectedGraph.AddEdge(0, 1);
            disconnectedGraph.AddEdge(2, 3);
            Assert.False(disconnectedGraph.IsGraphConnected());
        }

        [Fact]
        public void StringGraph_BFS_Works()
        {
            var g = new Graph<string>(false);
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            g.AddEdge("A", "D");
            g.AddEdge("D", "C");

            var traversal = g.BFS("A");
            Assert.Equal(4, traversal.Count);
            Assert.Equal("A", traversal[0]);
            Assert.Contains("B", traversal);
            Assert.Contains("C", traversal);
            Assert.Contains("D", traversal);

            var path = g.ShortestPathBFS("A", "C");
            Assert.Equal(3, path.Count);
            Assert.Equal("A", path[0]);
            Assert.Equal("C", path[2]);
        }
    }
}
