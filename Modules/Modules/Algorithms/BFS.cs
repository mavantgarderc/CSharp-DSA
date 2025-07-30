using Modules.DataStructures;

namespace Modules.Algorithms
{
    /// <summary>
    /// Breadth-First Search (BFS) Algorithms in C#
    /// Concepts Covered:
    ///     - Graph Traversal (BFS in adjacency list and matrix forms)
    ///     - Shortest Path Discovery in Unweighted Graphs
    ///     - Level-Order Traversal in Binary Trees
    ///     - BFS on Grids/Mazes (2D matrix - pathfinding & flood fill)
    /// Key Practices:
    ///     - Use of Queue for FIFO processing
    ///     - Visited node tracking to avoid cycles/repeats
    ///     - Layer/Level management for depth or step-based problems
    ///     - Early termination with predicates or goals (e.g., target found)

    public static class BFS
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
            public required TreeNode Left,
                Right;
        }
        #endregion


        public static int[] DistanceFromSource(Graph<int> g, int src)
        {
            int[] dist = new int[g.VertexCount];
            Array.Fill(dist, -1);
            Queue<int> q = new();

            dist[src] = 0;
            q.Enqueue(src);

            while (q.Count > 0)
            {
                int u = q.Dequeue();

                foreach (var v in g.Adj[u])
                {
                    if (dist[v] == -1)
                    {
                        dist[v] = dist[u] + 1;
                        q.Enqueue(v);
                    }
                }
            }
            return dist;
        }

        public static bool DetectCycleUndirected(Graph<int> g)
        {
            bool[] visited = new bool[g.VertexCount];
            for (int i = 0; i < g.VertexCount; i++)
            {
                if (!visited[i] && DetectCycleBFS(g, i, -1, visited))
                    return true;
            }
            return false;

            static bool DetectCycleBFS(Graph<int> g, int src, int parent, bool[] visited)
            {
                Queue<(int, int)> q = new();
                q.Enqueue((src, parent));
                visited[src] = true;

                while (q.Count > 0)
                {
                    var (u, p) = q.Dequeue();

                    foreach (var v in g.Adj[u])
                    {
                        if (!visited[v])
                        {
                            visited[v] = true;
                            q.Enqueue((v, u));
                        }
                        else if (v != p)
                            return true;
                    }
                }
                return false;
            }
        }

        public static List<int> TopologicalSortKahn(Graph<int> g)
        {
            int[] inDegree = new int[g.VertexCount];
            foreach (var u in g.Vertices)
                foreach (var v in g.Adj[u])
                    inDegree[v]++;

            Queue<int> q = new();
            foreach (var vertex in g.Vertices)
                if (inDegree[vertex] == 0)
                    q.Enqueue(vertex);

            List<int> topo = [];

            while (q.Count > 0)
            {
                int u = q.Dequeue();
                topo.Add(u);
                foreach (var v in g.Adj[u])
                {
                    inDegree[v]--;
                    if (inDegree[v] == 0)
                        q.Enqueue(v);
                }
            }
            return topo.Count == g.VertexCount ? topo : [];
        }

        public static int WordLadder(string beginWord, string endWord, List<string> wordList)
        {
            HashSet<string> words = new(wordList);
            if (!words.Contains(endWord))
                return 0;

            Queue<(string, int)> q = new();
            q.Enqueue((beginWord, 1));

            while (q.Count > 0)
            {
                var (word, depth) = q.Dequeue();

                if (word == endWord)
                    return depth;

                for (int i = 0; i < word.Length; i++)
                {
                    char[] arr = word.ToCharArray();

                    for (char c = 'a'; c <= 'z'; c++)
                    {
                        arr[i] = c;
                        string newWord = new(arr);
                        if (!words.Contains(newWord))
                        {
                            continue;
                        }
                        words.Remove(newWord);
                        q.Enqueue((newWord, depth + 1));
                    }
                }
            }
            return 0;
        }

        public static int[] MultiSourceBFS(Graph<int> g, List<int> sources)
        {
            int[] dist = new int[g.VertexCount];
            Array.Fill(dist, -1);
            Queue<int> q = new();

            foreach (var src in sources)
            {
                dist[src] = 0;
                q.Enqueue(src);
            }
            while (q.Count > 0)
            {
                int u = q.Dequeue();

                foreach (var v in g.Adj[u])
                {
                    if (dist[v] == -1)
                    {
                        dist[v] = dist[u] + 1;
                        q.Enqueue(v);
                    }
                }
            }
            return dist;
        }

        public static List<List<int>> ConnectedComponents(Graph<int> g)
        {
            bool[] visited = new bool[g.VertexCount];
            List<List<int>> components = [];

            foreach (var vertex in g.Vertices)
            {
                if (!visited[vertex])
                {
                    List<int> comp = [];
                    Queue<int> q = new();

                    q.Enqueue(vertex);
                    visited[vertex] = true;

                    while (q.Count > 0)
                    {
                        int u = q.Dequeue();
                        comp.Add(u);
                        foreach (var v in g.Adj[u])
                        {
                            if (!visited[v])
                            {
                                visited[v] = true;
                                q.Enqueue(v);
                            }
                        }
                    }

                    components.Add(comp);
                }
            }
            return components;
        }

        public static List<List<int>> AllPathsBFS(Graph<int> g, int src, int dst, int maxDepth)
        {
            List<List<int>> paths = [];
            Queue<List<int>> q = new();
            q.Enqueue([src]);

            while (q.Count > 0)
            {
                var path = q.Dequeue();

                if (path[^1] == dst && path.Count <= maxDepth)
                    paths.Add(new List<int>(path));

                if (path.Count >= maxDepth)
                    continue;

                foreach (var v in g.Adj[path[^1]])
                {
                    List<int> newPath = new(path) { v };
                    q.Enqueue(newPath);
                }
            }
            return paths;
        }

        public static int KnightsPath(int N, Point start, Point end)
        {
            int[] dx = [2, 2, -2, -2, 1, 1, -1, -1];
            int[] dy = [1, -1, 1, -1, 2, -2, 2, -2];

            bool[,] visited = new bool[N, N];
            Queue<(Point, int)> q = new();

            q.Enqueue((start, 0));
            visited[start.X, start.Y] = true;

            while (q.Count > 0)
            {
                var (p, depth) = q.Dequeue();

                if (p.X == end.X && p.Y == end.Y)
                    return depth;

                for (int i = 0; i < 8; i++)
                {
                    int nx = p.X + dx[i];
                    int ny = p.Y + dy[i];

                    if (nx >= 0 && nx < N && ny >= 0 && ny < N && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        q.Enqueue((new Point(nx, ny), depth + 1));
                    }
                }
            }
            return -1;
        }

        public static int BFSWithPredicate(Graph<int> g, int src, Func<int, bool> predicate)
        {
            bool[] visited = new bool[g.VertexCount];
            Queue<(int, int)> q = new();

            q.Enqueue((src, 0));
            visited[src] = true;

            while (q.Count > 0)
            {
                var (u, depth) = q.Dequeue();

                if (predicate(u))
                    return depth;

                foreach (var v in g.Adj[u])
                {
                    if (!visited[v])
                    {
                        visited[v] = true;
                        q.Enqueue((v, depth + 1));
                    }
                }
            }
            return -1;
        }

        public static int NumberOfIslands(int[,] grid)
        {
            int rows = grid.GetLength(0),
                cols = grid.GetLength(1);
            int islands = 0;
            bool[,] visited = new bool[rows, cols];
            int[] dx = [0, 1, 0, -1];
            int[] dy = [1, 0, -1, 0];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i, j] == 1 && !visited[i, j])
                    {
                        islands++;
                        Queue<Point> q = new();
                        q.Enqueue(new Point(i, j));
                        visited[i, j] = true;

                        while (q.Count > 0)
                        {
                            Point p = q.Dequeue();

                            for (int k = 0; k < 4; k++)
                            {
                                int nx = p.X + dx[k];
                                int ny = p.Y + dy[k];
                                if (
                                    nx >= 0
                                    && nx < rows
                                    && ny >= 0
                                    && ny < cols
                                    && grid[nx, ny] == 1
                                    && !visited[nx, ny]
                                )
                                {
                                    visited[nx, ny] = true;
                                    q.Enqueue(new Point(nx, ny));
                                }
                            }
                        }
                    }
                }
            }
            return islands;
        }

        public static int ShortestPathInGrid(int[,] grid, Point start, Point end)
        {
            int rows = grid.GetLength(0),
                cols = grid.GetLength(1);
            int[] dx = [0, 1, 0, -1];
            int[] dy = [1, 0, -1, 0];
            int[,] dist = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    dist[i, j] = -1;

            Queue<Point> q = new();

            q.Enqueue(start);
            dist[start.X, start.Y] = 0;

            while (q.Count > 0)
            {
                Point p = q.Dequeue();

                if (p.X == end.X && p.Y == end.Y)
                    return dist[p.X, p.Y];

                for (int k = 0; k < 4; k++)
                {
                    int nx = p.X + dx[k];
                    int ny = p.Y + dy[k];

                    if (
                        nx >= 0
                        && nx < rows
                        && ny >= 0
                        && ny < cols
                        && grid[nx, ny] == 0
                        && dist[nx, ny] == -1
                    )
                    {
                        dist[nx, ny] = dist[p.X, p.Y] + 1;
                        q.Enqueue(new Point(nx, ny));
                    }
                }
            }
            return -1;
        }
    }
}
