using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Algorithms;
using Modules.DataStructures;
using Xunit;

namespace Modules.Tests.Algorithms
{
    public class DFSTests
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

        private Graph<int> CreateDAG()
        {
            // 0 -> 1 -> 3
            //   \> 2 -/
            var g = new Graph<int>(true); // directed
            g.AddEdge(0, 1);
            g.AddEdge(1, 3);
            g.AddEdge(0, 2);
            g.AddEdge(2, 3);
            return g;
        }

        [Fact]
        public void DFS_Iterative_Works()
        {
            var g = CreateSimpleGraph();
            var traversal = g.DFS(0);
            Assert.Equal(4, traversal.Count);
            Assert.Equal(0, traversal[0]);
            Assert.Contains(1, traversal);
            Assert.Contains(2, traversal);
            Assert.Contains(3, traversal);
        }

        [Fact]
        public void DFS_Recursive_Works()
        {
            var g = CreateSimpleGraph();
            var traversal = g.DFS_Recursive(0);
            Assert.Equal(4, traversal.Count);
            Assert.Equal(0, traversal[0]);
            Assert.Contains(1, traversal);
            Assert.Contains(2, traversal);
            Assert.Contains(3, traversal);
        }

        [Fact]
        public void TopologicalSort_Works()
        {
            var g = CreateDAG();
            var topo = g.TopologicalSort();
            Assert.Equal(4, topo.Count);
            Assert.True(topo.IndexOf(0) < topo.IndexOf(1));
            Assert.True(topo.IndexOf(0) < topo.IndexOf(2));
            Assert.True(topo.IndexOf(1) < topo.IndexOf(3));
            Assert.True(topo.IndexOf(2) < topo.IndexOf(3));
        }

        [Fact]
        public void TopologicalSort_ThrowsOnCycle()
        {
            var g = new Graph<int>(true);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);
            Assert.Throws<Exception>(() => g.TopologicalSort());
        }

        [Fact]
        public void HasCycle_DetectsDirectedCycle()
        {
            var acyclicGraph = CreateDAG();
            Assert.False(acyclicGraph.HasCycle());

            var cyclicGraph = new Graph<int>(true);
            cyclicGraph.AddEdge(0, 1);
            cyclicGraph.AddEdge(1, 2);
            cyclicGraph.AddEdge(2, 0);
            Assert.True(cyclicGraph.HasCycle());
        }

        [Fact]
        public void HasCycle_DetectsUndirectedCycle()
        {
            var acyclicGraph = new Graph<int>(false);
            acyclicGraph.AddEdge(0, 1);
            acyclicGraph.AddEdge(1, 2);
            Assert.False(acyclicGraph.HasCycle());

            var cyclicGraph = CreateSimpleGraph();
            Assert.True(cyclicGraph.HasCycle());
        }

        [Fact]
        public void FindCycle_ReturnsValidCycle()
        {
            var g = new Graph<int>(true);
            g.AddEdge(0, 1);
            g.AddEdge(1, 2);
            g.AddEdge(2, 0);

            var cycle = g.FindCycle();
            Assert.True(cycle.Count >= 3);
            Assert.Contains(0, cycle);
            Assert.Contains(1, cycle);
            Assert.Contains(2, cycle);
        }

        [Fact]
        public void FindCycle_ReturnsEmptyForAcyclicGraph()
        {
            var g = CreateDAG();
            var cycle = g.FindCycle();
            Assert.Empty(cycle);
        }

        [Fact]
        public void FindAllPaths_Works()
        {
            var g = CreateDAG();
            var paths = g.FindAllPaths(0, 3);
            Assert.Equal(2, paths.Count);

            var expectedPaths = new List<List<int>>
            {
                new() { 0, 1, 3 },
                new() { 0, 2, 3 },
            };

            Assert.All(
                expectedPaths,
                expectedPath => Assert.Contains(paths, path => path.SequenceEqual(expectedPath))
            );
        }

        [Fact]
        public void CountPaths_Works()
        {
            var g = CreateDAG();
            int pathCount = g.CountPaths(0, 3);
            Assert.Equal(2, pathCount);

            Assert.Equal(0, g.CountPaths(0, 99));
        }

        [Fact]
        public void DFS_EmptyGraph_Works()
        {
            var g = new Graph<int>(false);
            g.AddVertex(0);
            var traversal = g.DFS(0);
            Assert.Single(traversal);
            Assert.Equal(0, traversal[0]);
        }

        [Fact]
        public void DFS_SingleEdge_Works()
        {
            var g = new Graph<int>(false);
            g.AddEdge(0, 1);
            var traversal = g.DFS(0);
            Assert.Equal(2, traversal.Count);
            Assert.Equal(0, traversal[0]);
            Assert.Contains(1, traversal);
        }

        [Fact]
        public void StringGraph_DFS_Works()
        {
            var g = new Graph<string>(false);
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            g.AddEdge("A", "D");

            var traversal = g.DFS("A");
            Assert.Equal(4, traversal.Count);
            Assert.Equal("A", traversal[0]);
            Assert.Contains("B", traversal);
            Assert.Contains("C", traversal);
            Assert.Contains("D", traversal);

            var paths = g.FindAllPaths("A", "C");
            Assert.Single(paths);
            Assert.Equal(new[] { "A", "B", "C" }, paths[0]);
        }

        [Fact]
        public void ComplexGraph_DFS_Works()
        {
            var g = new Graph<int>(true);
            g.AddEdge(0, 1);
            g.AddEdge(0, 2);
            g.AddEdge(1, 3);
            g.AddEdge(1, 4);
            g.AddEdge(2, 4);
            g.AddEdge(2, 5);
            g.AddEdge(3, 6);
            g.AddEdge(4, 6);
            g.AddEdge(5, 6);

            var topo = g.TopologicalSort();
            Assert.Equal(7, topo.Count);

            Assert.True(topo.IndexOf(0) < topo.IndexOf(1));
            Assert.True(topo.IndexOf(0) < topo.IndexOf(2));
            Assert.True(topo.IndexOf(1) < topo.IndexOf(3));
            Assert.True(topo.IndexOf(1) < topo.IndexOf(4));
            Assert.True(topo.IndexOf(2) < topo.IndexOf(4));
            Assert.True(topo.IndexOf(2) < topo.IndexOf(5));
            Assert.True(topo.IndexOf(3) < topo.IndexOf(6));
            Assert.True(topo.IndexOf(4) < topo.IndexOf(6));
            Assert.True(topo.IndexOf(5) < topo.IndexOf(6));

            int pathsTo6 = g.CountPaths(0, 6);
            Assert.True(pathsTo6 > 1);
        }

        [Fact]
        public void DFS_StaticMethods_Integration()
        {
            var g = CreateSimpleGraph();

            Assert.True(g.IsGraphConnected());
            Assert.Equal(1, g.CountConnectedComponents());
        }
    }
}
