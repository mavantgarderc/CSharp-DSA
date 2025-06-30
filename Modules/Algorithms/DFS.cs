using Modules.DataStructures;

namespace Modules.Algorithms
{
    /// <summary>
    /// Concepts:
    ///     - graph Traversal (DFS, both iterative and recursive)
    ///     - path Existence and All Paths Discovery
    ///     - cycle Detection (directed and undirected graphs)
    ///     - topological Sort (DFS-based)
    ///     - connected Components
    ///     - flood Fill / Island Counting on 2D grids
    ///     - dFS on Trees (preorder, postorder)
    /// Key Practices:
    ///     - use of Stack for explicit DFS traversal
    ///     - visited node tracking to avoid cycles/repeats
    ///     - early termination with predicates/goals
    ///     - layer/Level/depth management for path-based problems
    ///     - recursive and iterative paradigms
    /// </summary>

    public static class DFS
    {
        #region Helpers
        public readonly struct Point(int x, int y)
        {
            public int X { get; } = x;
            public int Y { get; } = y;
        }

        public class TreeNode
        {
            public int Val;
            public required TreeNode Left, Right;
        }
        #endregion

        // classic DFS traversal (returns order)
        public static List<int> DFSOrder(Graph<int> g, int src)
        {
            var order = new List<int>();
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(src);

            while (stack.Count > 0)
            {
                int u = stack.Pop();
                if (visited.Add(u))
                {
                    order.Add(u);
                    foreach (var v in g.Adj[u].AsEnumerable().Reverse())
                        if (!visited.Contains(v))
                            stack.Push(v);
                }
            }
            return order;
        }

        // path existence (is there a path from src to dst)
        public static bool HasPath(Graph<int> g, int src, int dst)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            stack.Push(src);

            while (stack.Count > 0)
            {
                int u = stack.Pop();
                if (u == dst) return true;
                if (visited.Add(u))
                    foreach (var v in g.Adj[u])
                        if (!visited.Contains(v))
                            stack.Push(v);
            }
            return false;
        }

        // all simple paths from src to dst, up to maxDepth
        public static List<List<int>> AllPathsDFS(Graph<int> g, int src, int dst, int maxDepth = int.MaxValue)
        {
            var result = new List<List<int>>();
            var path = new List<int>();
            void Dfs(int u, int depth)
            {
                path.Add(u);
                if (u == dst && path.Count <= maxDepth)
                    result.Add(new List<int>(path));
                else if (depth < maxDepth)
                {
                    foreach (var v in g.Adj[u])
                        if (!path.Contains(v))
                            Dfs(v, depth + 1);
                }
                path.RemoveAt(path.Count - 1);
            }
            Dfs(src, 1);
            return result;
        }

        // cycle Detection for Directed Graphs
        public static bool HasCycleDirected(Graph<int> g)
        {
            var visited = new HashSet<int>();
            var stack = new HashSet<int>();
            bool Dfs(int u)
            {
                if (stack.Contains(u)) return true;
                if (!visited.Add(u)) return false;
                stack.Add(u);
                foreach (var v in g.Adj[u])
                    if (Dfs(v))
                        return true;
                stack.Remove(u);
                return false;
            }
            foreach (var node in g.Adj.Keys)
                if (Dfs(node))
                    return true;
            return false;
        }

        // cycle Detection for Undirected Graphs
        public static bool HasCycleUndirected(Graph<int> g)
        {
            var visited = new HashSet<int>();
            bool Dfs(int u, int parent)
            {
                visited.Add(u);
                foreach (var v in g.Adj[u])
                {
                    if (!visited.Contains(v))
                    {
                        if (Dfs(v, u)) return true;
                    }
                    else if (v != parent)
                    {
                        return true;
                    }
                }
                return false;
            }
            foreach (var node in g.Adj.Keys)
                if (!visited.Contains(node) && Dfs(node, -1))
                    return true;
            return false;
        }

        // topological Sort (DFS based)
        public static List<int> TopologicalSortDFS(Graph<int> g)
        {
            var visited = new HashSet<int>();
            var result = new List<int>();
            void Dfs(int u)
            {
                visited.Add(u);
                foreach (var v in g.Adj[u])
                    if (!visited.Contains(v))
                        Dfs(v);
                result.Add(u);
            }
            foreach (var u in g.Adj.Keys)
                if (!visited.Contains(u))
                    Dfs(u);
            result.Reverse();
            return result;
        }

        // connected Components (undirected)
        public static List<List<int>> ConnectedComponents(Graph<int> g)
        {
            var visited = new HashSet<int>();
            var components = new List<List<int>>();
            foreach (var u in g.Adj.Keys)
            {
                if (!visited.Contains(u))
                {
                    var comp = new List<int>();
                    void Dfs(int v)
                    {
                        visited.Add(v);
                        comp.Add(v);
                        foreach (var w in g.Adj[v])
                            if (!visited.Contains(w))
                                Dfs(w);
                    }
                    Dfs(u);
                    components.Add(comp);
                }
            }
            return components;
        }

        // DFS with predicate (returns depth for first match, else -1)
        public static int DFSWithPredicate(Graph<int> g, int src, Func<int, bool> predicate)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<(int, int)>();
            stack.Push((src, 0));
            while (stack.Count > 0)
            {
                var (u, depth) = stack.Pop();
                if (!visited.Add(u)) continue;
                if (predicate(u)) return depth;
                foreach (var v in g.Adj[u])
                    if (!visited.Contains(v))
                        stack.Push((v, depth + 1));
            }
            return -1;
        }

        // Flood Fill (2D DFS)
        public static void FloodFill(int[,] grid, Point start, int newColor)
        {
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            int orig = grid[start.X, start.Y];
            if (orig == newColor) return;
            void Dfs(int x, int y)
            {
                if (x < 0 || y < 0 || x >= rows || y >= cols) return;
                if (grid[x, y] != orig) return;
                grid[x, y] = newColor;
                Dfs(x + 1, y); Dfs(x - 1, y); Dfs(x, y + 1); Dfs(x, y - 1);
            }
            Dfs(start.X, start.Y);
        }

        // number of Islands in 2D grid
        public static int NumberOfIslandsDFS(int[,] grid)
        {
            int rows = grid.GetLength(0), cols = grid.GetLength(1);
            bool[,] visited = new bool[rows, cols];
            int count = 0;
            void Dfs(int x, int y)
            {
                if (x < 0 || y < 0 || x >= rows || y >= cols) return;
                if (grid[x, y] != 1 || visited[x, y]) return;
                visited[x, y] = true;
                Dfs(x + 1, y); Dfs(x - 1, y); Dfs(x, y + 1); Dfs(x, y - 1);
            }
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (grid[i, j] == 1 && !visited[i, j])
                    {
                        count++;
                        Dfs(i, j);
                    }
            return count;
        }

        // preorder traversal of binary tree
        public static List<int> Preorder(TreeNode root)
        {
            var result = new List<int>();
            void Dfs(TreeNode node)
            {
                if (node == null) return;
                result.Add(node.Val);
                if (node.Left != null) Dfs(node.Left);
                if (node.Right != null) Dfs(node.Right);
            }
            Dfs(root);
            return result;
        }

        // postorder traversal of binary tree
        public static List<int> Postorder(TreeNode root)
        {
            var result = new List<int>();
            void Dfs(TreeNode node)
            {
                if (node == null) return;
                if (node.Left != null) Dfs(node.Left);
                if (node.Right != null) Dfs(node.Right);
                result.Add(node.Val);
            }
            Dfs(root);
            return result;
        }
    }
}
