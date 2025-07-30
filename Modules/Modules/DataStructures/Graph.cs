using System.Text;
using System.Text.Json;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Graph Representation (using adjacency lists for sparse graphs).
    ///     - Algorithm Variety (traversals, path-finding, coloring, MST, etc.).
    ///     - Directed vs Undirected Operations (supports both, depending on usage).
    ///     - Algorithmic Complexity (leveraging BFS, DFS, Dijkstra, Bellman-Ford, etc.).
    /// Key Practices:
    ///     - Encapsulation (keeping adj private and accessing through methods).
    ///     - Reusable Methods (self-contained methods for each algorithm).
    ///     - Exception Handling (throws when coloring fails or negative cycles are present).
    ///     - Forward-thinking Design (supports expansion to advanced algorithms like flow or centroid).
    ///     - Generic support
    ///     - Serialization support
    ///     - Performance optimization
    ///     - Advanced Graph Operations
    /// </summary>
    public class Graph<T>
        where T : IComparable<T>, IEquatable<T>
    {
        #region Construction

        private readonly Dictionary<T, List<(T vertex, int weight)>> adj = [];
        private int? _cachedVertexCount;
        private bool _isDirty = true;
        public event Action? GraphChanged;
        public bool IsDirected { get; private set; } = true;

        /// <summary>
        /// exposes the graph as an adjacency list of nodes only, for BFS-style algorithms
        /// each node is mapped to a List of its neighbors (ignoring weights)
        /// for compatibility with algorithms that expect List<int>[] style access
        /// </summary>
        public Dictionary<T, List<T>> Adj
        {
            get
            {
                var dict = new Dictionary<T, List<T>>();
                foreach (var kvp in adj)
                {
                    dict[kvp.Key] = kvp.Value.Select(e => e.vertex).ToList();
                }
                return dict;
            }
        }

        public Graph(bool isDirected = true)
        {
            IsDirected = isDirected;
        }

        private void MarkDirty()
        {
            _isDirty = true;
            GraphChanged?.Invoke();
        }

        public List<T> GetNeighbors(T vertex)
        {
            return adj.TryGetValue(vertex, out var value) ? value.Select(e => e.vertex).ToList() : [];
        }

        public List<(T vertex, int weight)> GetNeighborsWithWeights(T vertex)
        {
            return adj.TryGetValue(vertex, out var value) ? value.ToList() : [];
        }

        public bool HasEdge(T a, T b)
        {
            return adj.ContainsKey(a) && adj[a].Any(e => e.vertex.Equals(b));
        }

        public bool IsConnected(T a, T b)
        {
            return HasEdge(a, b);
        }

        public int VertexCount
        {
            get
            {
                if (_isDirty || !_cachedVertexCount.HasValue)
                {
                    _cachedVertexCount = adj.Count;
                    _isDirty = false;
                }
                return _cachedVertexCount.Value;
            }
        }

        public int EdgeCount => adj.Values.Sum(list => list.Count) / (IsDirected ? 1 : 2);

        public IEnumerable<T> Vertices => adj.Keys;

        public IEnumerable<(T src, T dst, int weight)> Edges =>
            adj.SelectMany(kvp => kvp.Value.Select(e => (kvp.Key, e.vertex, e.weight)));

        public void AddVertex(T vertex)
        {
            if (!adj.ContainsKey(vertex))
            {
                adj[vertex] = [];
                MarkDirty();
            }
        }

        public void RemoveVertex(T vertex)
        {
            if (adj.Remove(vertex))
            {
                foreach (var kvp in adj)
                {
                    kvp.Value.RemoveAll(e => e.vertex.Equals(vertex));
                }
                MarkDirty();
            }
        }

        public void AddEdge(T src, T dst, int weight = 1)
        {
            AddVertex(src);
            AddVertex(dst);

            if (!adj[src].Any(e => e.vertex.Equals(dst)))
            {
                adj[src].Add((dst, weight));

                if (!IsDirected && !src.Equals(dst))
                {
                    adj[dst].Add((src, weight));
                }
                MarkDirty();
            }
        }

        public void RemoveEdge(T src, T dst)
        {
            if (!adj.ContainsKey(src))
                return;

            bool removed = adj[src].RemoveAll(e => e.vertex.Equals(dst)) > 0;

            if (!IsDirected && adj.ContainsKey(dst))
            {
                removed |= adj[dst].RemoveAll(e => e.vertex.Equals(src)) > 0;
            }

            if (removed)
                MarkDirty();
        }

        #endregion


        #region misc

        public void ReverseGraph()
        {
            var reversed = new Dictionary<T, List<(T vertex, int weight)>>();

            foreach (var kvp in adj)
            {
                var src = kvp.Key;
                foreach (var (dst, weight) in kvp.Value)
                {
                    if (!reversed.ContainsKey(dst))
                    {
                        reversed[dst] = [];
                    }
                    reversed[dst].Add((src, weight));
                }
            }

            // Actually apply the reversed graph
            adj.Clear();
            foreach (var kvp in reversed)
            {
                adj[kvp.Key] = kvp.Value;
            }
            MarkDirty();
        }

        public string ToDot()
        {
            var sb = new StringBuilder(IsDirected ? "digraph G {\n" : "graph G {\n");

            foreach (var kvp in adj)
            {
                foreach (var (dst, weight) in kvp.Value)
                {
                    string connector = IsDirected ? "->" : "--";
                    sb.AppendLine($"    {kvp.Key} {connector} {dst} [label=\"{weight}\"];");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        public string ToJson()
        {
            var graphData = new
            {
                IsDirected = IsDirected,
                Vertices = adj.Keys.ToList(),
                Edges = Edges.ToList(),
            };
            return JsonSerializer.Serialize(
                graphData,
                new JsonSerializerOptions { WriteIndented = true }
            );
        }

        public Graph<T> Clone()
        {
            var clone = new Graph<T>(IsDirected);

            foreach (var (src, dst, weight) in Edges)
            {
                clone.AddEdge(src, dst, weight);
            }
            return clone;
        }

        #endregion


        #region Traversals

        // BFS
        public List<T> BFS(T start)
        {
            List<T> result = [];
            HashSet<T> visited = [];
            Queue<T> q = new();

            visited.Add(start);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var node = q.Dequeue();
                result.Add(node);
                foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        q.Enqueue(neighbor);
                    }
                }
            }
            return result;
        }

        // DFS (iterative)
        public List<T> DFS(T start)
        {
            List<T> result = [];
            HashSet<T> visited = [];
            Stack<T> stack = new();

            stack.Push(start);

            while (stack.Count > 0)
            {
                var node = stack.Pop();
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    result.Add(node);

                    // Add neighbors in reverse order to maintain left-to-right traversal
                    var neighbors = adj.GetValueOrDefault(node, []).Select(e => e.vertex).ToList();
                    for (int i = neighbors.Count - 1; i >= 0; i--)
                    {
                        if (!visited.Contains(neighbors[i]))
                        {
                            stack.Push(neighbors[i]);
                        }
                    }
                }
            }
            return result;
        }

        // DFS (recursive) helper
        private void DFSUtil(T node, HashSet<T> visited, List<T> result)
        {
            visited.Add(node);
            result.Add(node);
            foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
            {
                if (!visited.Contains(neighbor))
                    DFSUtil(neighbor, visited, result);
            }
        }

        public List<T> DFS_Recursive(T start)
        {
            List<T> result = [];
            HashSet<T> visited = [];
            DFSUtil(start, visited, result);
            return result;
        }

        #endregion


        #region Topological Sort

        // Topological Sort (DFS)
        public List<T> TopologicalSort()
        {
            var visited = new HashSet<T>();
            var recursionStack = new HashSet<T>();
            var result = new List<T>();

            foreach (var node in adj.Keys)
            {
                if (!visited.Contains(node))
                {
                    DfsForTopSort(node, visited, recursionStack, result);
                }
            }
            result.Reverse();
            return result;
        }

        private void DfsForTopSort(
            T node,
            HashSet<T> visited,
            HashSet<T> recursionStack,
            List<T> result
        )
        {
            if (recursionStack.Contains(node))
                throw new Exception("Graph contains a cycle.");

            if (visited.Contains(node))
                return;

            recursionStack.Add(node);
            visited.Add(node);

            foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
            {
                DfsForTopSort(neighbor, visited, recursionStack, result);
            }

            recursionStack.Remove(node);
            result.Add(node);
        }

        #endregion


        #region Paths

        // All paths
        public List<List<T>> FindAllPaths(T start, T end)
        {
            List<List<T>> paths = [];
            List<T> path = [];

            void DFS(T node)
            {
                path.Add(node);
                if (node.Equals(end))
                {
                    paths.Add(new List<T>(path));
                }
                else
                {
                    foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                    {
                        if (!path.Contains(neighbor))
                            DFS(neighbor);
                    }
                }
                path.RemoveAt(path.Count - 1);
            }
            DFS(start);
            return paths;
        }

        // Count paths
        public int CountPaths(T start, T end)
        {
            int count = 0;
            HashSet<T> path = [];

            void DFS(T node)
            {
                path.Add(node);
                if (node.Equals(end))
                {
                    count++;
                }
                else
                {
                    foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                    {
                        if (!path.Contains(neighbor))
                            DFS(neighbor);
                    }
                }
                path.Remove(node);
            }
            DFS(start);
            return count;
        }

        public List<T> ShortestPathBFS(T start, T end)
        {
            // unweighted
            var parent = new Dictionary<T, T?>();
            var queue = new Queue<T>([start]);
            var visited = new HashSet<T> { start };
            parent[start] = default(T);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node.Equals(end))
                {
                    break;
                }

                foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                {
                    if (visited.Add(neighbor))
                    {
                        parent[neighbor] = node;
                        queue.Enqueue(neighbor);
                    }
                }
            }
            if (!parent.ContainsKey(end))
                return [];

            var path = new List<T>();
            var curr = end;

            while (!curr.Equals(start))
            {
                path.Add(curr);
                curr = parent[curr];
            }
            path.Add(start);
            path.Reverse();
            return path;
        }

        public List<T> Dijkstra(T start, T end)
        {
            var dist = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            var prev = new Dictionary<T, T>();
            var pq = new SortedSet<(int distance, T vertex)>(
                Comparer<(int, T)>.Create(
                    (a, b) =>
                    {
                        int cmp = a.Item1.CompareTo(b.Item1);
                        return cmp == 0 ? a.Item2.CompareTo(b.Item2) : cmp;
                    }
                )
            );

            dist[start] = 0;
            pq.Add((0, start));

            while (pq.Count > 0)
            {
                var (d, vertex) = pq.Min;
                pq.Remove(pq.Min);

                if (d > dist[vertex])
                {
                    continue;
                }

                if (vertex.Equals(end))
                {
                    break;
                }

                foreach (var (neighbor, weight) in adj.GetValueOrDefault(vertex, []))
                {
                    int newDist = d + weight;

                    if (newDist < dist[neighbor])
                    {
                        if (dist[neighbor] != int.MaxValue)
                        {
                            pq.Remove((dist[neighbor], neighbor));
                        }
                        dist[neighbor] = newDist;
                        prev[neighbor] = vertex;
                        pq.Add((newDist, neighbor));
                    }
                }
            }

            if (!prev.ContainsKey(end))
            {
                return [];
            }

            var path = new List<T>();
            var curr = end;

            while (prev.ContainsKey(curr))
            {
                path.Add(curr);
                curr = prev[curr];
            }

            path.Add(start);
            path.Reverse();
            return path;
        }

        #endregion


        #region Cycle

        public bool HasCycle()
        {
            if (IsDirected)
            {
                return HasCycleDirectedHelper();
            }
            else
            {
                return HasCycleUndirectedHelper();
            }
        }

        private bool HasCycleDirectedHelper()
        {
            var visited = new HashSet<T>();
            var recursionStack = new HashSet<T>();

            bool Dfs(T node)
            {
                if (recursionStack.Contains(node))
                {
                    return true;
                }
                if (visited.Contains(node))
                {
                    return false;
                }

                visited.Add(node);
                recursionStack.Add(node);

                foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                {
                    if (Dfs(neighbor))
                    {
                        return true;
                    }
                }

                recursionStack.Remove(node);
                return false;
            }

            foreach (var node in adj.Keys)
            {
                if (Dfs(node))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasCycleUndirectedHelper()
        {
            var visited = new HashSet<T>();

            bool Dfs(T node, T parent)
            {
                visited.Add(node);

                foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                {
                    if (!visited.Contains(neighbor))
                    {
                        if (Dfs(neighbor, node))
                            return true;
                    }
                    else if (!neighbor.Equals(parent))
                    {
                        return true;
                    }
                }
                return false;
            }

            foreach (var node in adj.Keys)
            {
                if (!visited.Contains(node))
                {
                    if (Dfs(node, default(T)!))
                        return true;
                }
            }
            return false;
        }

        [Obsolete("Use HasCycle instead!")]
        public bool HasCycleDirected() => HasCycle();

        public List<T> FindCycle()
        {
            var parent = new Dictionary<T, T?>();
            var recursionStack = new HashSet<T>();

            List<T> cycle = [];

            bool Dfs(T node)
            {
                recursionStack.Add(node);

                foreach (var (neighbor, _) in adj.GetValueOrDefault(node, []))
                {
                    if (!parent.ContainsKey(neighbor))
                    {
                        parent[neighbor] = node;

                        if (Dfs(neighbor))
                        {
                            return true;
                        }
                    }
                    else if (recursionStack.Contains(neighbor))
                    {
                        var current = node;
                        cycle.Add(neighbor);

                        while (!current.Equals(neighbor))
                        {
                            cycle.Add(current);
                            current = parent[current]!;
                        }
                        cycle.Reverse();

                        return true;
                    }
                }
                recursionStack.Remove(node);
                return false;
            }

            foreach (var node in adj.Keys)
            {
                if (!parent.ContainsKey(node))
                {
                    parent[node] = default(T);

                    if (Dfs(node))
                    {
                        return cycle;
                    }
                }
            }
            return [];
        }

        #endregion


        #region Connectivity

        public bool IsGraphConnected()
        {
            if (adj.Keys.Count == 0)
            {
                return true;
            }

            var start = adj.Keys.First();
            var reached = BFS(start);

            return reached.Count == adj.Keys.Count;
        }

        public List<List<T>> ConnectedComponents()
        {
            var components = new List<List<T>>();
            var visited = new HashSet<T>();

            foreach (var node in adj.Keys)
            {
                if (visited.Contains(node))
                    continue;

                var comp = BFS(node);
                components.Add(comp);
                foreach (var n in comp)
                    visited.Add(n);
            }
            return components;
        }

        public int CountConnectedComponents()
        {
            return ConnectedComponents().Count;
        }

        #endregion


        #region Graph Coloring

        public Dictionary<T, int> GraphColoring(int maxColors)
        {
            var nodes = adj.Keys.ToList();
            var colors = new Dictionary<T, int>();

            // Initialize all colors to -1 (uncolored)
            foreach (var node in nodes)
            {
                colors[node] = -1;
            }

            // Color each node
            foreach (var node in nodes)
            {
                var unavailable = new HashSet<int>();

                // Check colors of all neighbors (both outgoing and incoming edges)
                var allNeighbors = new HashSet<T>();

                // Add outgoing neighbors
                foreach (var neighbor in GetNeighbors(node))
                {
                    allNeighbors.Add(neighbor);
                }

                // Add incoming neighbors (nodes that point to this node)
                foreach (var kvp in adj)
                {
                    if (kvp.Value.Any(e => e.vertex.Equals(node)))
                    {
                        allNeighbors.Add(kvp.Key);
                    }
                }

                // Check colors of all connected neighbors
                foreach (var neighbor in allNeighbors)
                {
                    if (colors.ContainsKey(neighbor) && colors[neighbor] != -1)
                    {
                        unavailable.Add(colors[neighbor]);
                    }
                }

                // Find the smallest available color
                int color = 0;
                while (unavailable.Contains(color))
                {
                    color++;
                }

                // Check if we exceeded max colors
                if (color >= maxColors)
                {
                    throw new Exception("Impossible Coloring...");
                }

                colors[node] = color;
            }

            return colors;
        }
        #endregion


        #region AStar

        public List<T> AStar(T start, T end, Func<T, T, int>? heuristic = null)
        {
            heuristic ??= (a, b) => 0;
            var gScore = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            gScore[start] = 0;

            var fScore = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            fScore[start] = heuristic(start, end);

            var cameFrom = new Dictionary<T, T>();

            var open = new SortedSet<(int score, T vertex)>(
                    Comparer<(int, T)>.Create(
                        (a, b) =>
                        {
                        int cmp = a.Item1.CompareTo(b.Item1);
                        return cmp == 0 ? a.Item2.CompareTo(b.Item2) : cmp;
                        }
                        )
                    )
            {
                (fScore[start], start),
            };

            while (open.Count > 0)
            {
                var (currentScore, current) = open.Min;

                if (current.Equals(end))
                {
                    break;
                }

                open.Remove(open.Min);

                foreach (var (neighbor, weight) in adj.GetValueOrDefault(current, []))
                {
                    int tentativeG = gScore[current] + weight;

                    if (tentativeG < gScore[neighbor])
                    {
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + heuristic(neighbor, end);
                        cameFrom[neighbor] = current;

                        if (!open.Contains((fScore[neighbor], neighbor)))
                        {
                            open.Add((fScore[neighbor], neighbor));
                        }
                    }
                }
            }

            if (!cameFrom.ContainsKey(end))
            {
                return [];
            }

            var path = new List<T>();
            var curr = end;

            while (cameFrom.ContainsKey(curr))
            {
                path.Add(curr);
                curr = cameFrom[curr];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }

        #endregion


        #region Bellman-Ford

        public List<T> BellmanFord(T start, T end)
        {
            var dist = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            var pred = new Dictionary<T, T>();

            dist[start] = 0;

            for (int i = 0; i < adj.Keys.Count - 1; i++)
            {
                foreach (var u in adj.Keys)
                {
                    if (dist[u] == int.MaxValue)
                    {
                        continue;
                    }

                    foreach (var (v, w) in adj[u])
                    {
                        if (dist[u] + w < dist[v])
                        {
                            dist[v] = dist[u] + w;
                            pred[v] = u;
                        }
                    }
                }
            }

            foreach (var u in adj.Keys)
            {
                if (dist[u] == int.MaxValue)
                {
                    continue;
                }

                foreach (var (v, w) in adj[u])
                {
                    if (dist[u] + w < dist[v])
                    {
                        throw new Exception("Graph contains a negative-weight cycle...");
                    }
                }
            }

            if (!pred.ContainsKey(end))
            {
                return [];
            }

            var path = new List<T>();
            var curr = end;

            while (pred.ContainsKey(curr))
            {
                path.Add(curr);
                curr = pred[curr];
            }
            path.Add(start);
            path.Reverse();

            return path;
        }

        #endregion


        #region MST

        public List<(T src, T dst, int weight)> KruskalMST()
        {
            var edges = adj.SelectMany(kvp => kvp.Value.Select(e => (kvp.Key, e.vertex, e.weight)))
                .ToList();
            edges.Sort((a, b) => a.weight.CompareTo(b.weight));

            var parent = adj.Keys.ToDictionary(k => k, k => k);

            T Find(T x)
            {
                if (!parent[x].Equals(x))
                {
                    parent[x] = Find(parent[x]); // Path compression
                }
                return parent[x];
            }

            void Union(T a, T b)
            {
                T rootA = Find(a);
                T rootB = Find(b);

                if (!rootA.Equals(rootB))
                {
                    parent[rootB] = rootA;
                }
            }

            List<(T, T, int)> mst = [];

            foreach (var (u, v, w) in edges)
            {
                if (!Find(u).Equals(Find(v)))
                {
                    mst.Add((u, v, w));
                    Union(u, v);
                }
            }

            return mst;
        }

        public List<(T src, T dst, int weight)> PrimMST()
        {
            if (adj.Count == 0)
            {
                return [];
            }

            var inMST = new HashSet<T>();

            var pq = new SortedSet<(int weight, T src, T dst)>(
                    Comparer<(int, T, T)>.Create(
                        (a, b) =>
                        {
                        int cmp = a.Item1.CompareTo(b.Item1);
                        if (cmp == 0)
                        {
                        cmp = a.Item2.CompareTo(b.Item2);
                        }
                        if (cmp == 0)
                        {
                        cmp = a.Item3.CompareTo(b.Item3);
                        }
                        return cmp;
                        }
                        )
                    );

            T start = adj.Keys.First();
            inMST.Add(start);

            foreach (var (neighbor, weight) in adj[start])
            {
                pq.Add((weight, start, neighbor));
            }

            List<(T, T, int)> mst = [];

            while (pq.Count > 0 && inMST.Count < adj.Keys.Count)
            {
                var (w, u, v) = pq.Min;
                pq.Remove(pq.Min);

                if (inMST.Contains(v))
                {
                    continue;
                }

                inMST.Add(v);
                mst.Add((u, v, w));

                foreach (var (neighbor, weight) in adj[v])
                {
                    if (!inMST.Contains(neighbor))
                    {
                        pq.Add((weight, v, neighbor));
                    }
                }
            }

            return mst;
        }

        #endregion
    }
}
