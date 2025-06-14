using System.Text;

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
    /// </summary>
    public class Graph
    {
        #region Construction

        private readonly Dictionary<int, List<(int, int)>> adj = [];

        public void AddNode(int node)
        {
            if (!adj.ContainsKey(node))
                adj[node] = [];
        }

        public void RemoveNode(int node)
        {
            adj.Remove(node);

            foreach (var kvp in adj)
            {
                kvp.Value.RemoveAll(e => e.Item1 == node);
            }
        }

        public void AddEdge(int src, int dst, int weight = 1)
        {
            AddNode(src);
            AddNode(dst);
            adj[src].Add((dst, weight));
        }

        public void RemoveEdge(int src, int dst)
        {
            adj[src].RemoveAll(e => e.Item1 == dst);
        }

        public List<int> GetNeighbors(int node) =>
            adj.TryGetValue(node, out List<(int, int)>? value)
                ? value.Select(e => e.Item1).ToList()
                : [];

        public bool IsConnected(int a, int b) =>
            adj.ContainsKey(a) && adj[a].Any(e => e.Item1 == b);

        #endregion


        #region misc

        public void ReverseGraph()
        {
            var reversed = new Dictionary<int, List<(int, int)>>();

            foreach (var kvp in adj)
            {
                int src = kvp.Key;

                foreach (var (dst, w) in kvp.Value)
                {
                    if (!reversed.ContainsKey(dst))
                        reversed[dst] = [];
                    reversed[dst].Add((src, w)); // reverse direction
                }
            }
            adj.Clear();
            foreach (var kvp in reversed)
            {
                adj[kvp.Key] = kvp.Value;
            }
        }

        public string ToDot()
        {
            var sb = new StringBuilder("digraph G {\n");

            foreach (var kvp in adj)
            {
                foreach (var (dst, _) in kvp.Value)
                {
                    sb.AppendLine($"    {kvp.Key} => {dst};");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        #endregion


        #region Traversals

        // BFS
        public List<int> BFS(int start)
        {
            List<int> result = [];
            HashSet<int> visited = [];
            Queue<int> q = new();

            visited.Add(start);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                int node = q.Dequeue();
                result.Add(node);
                foreach (var (neighbor, _) in adj[node])
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }
                    visited.Add(neighbor);
                    q.Enqueue(neighbor);
                }
            }
            return result;
        }

        // DFS (iterative)
        public List<int> DFS(int start)
        {
            List<int> result = [];
            HashSet<int> visited = [];
            Stack<int> stack = new();

            stack.Push(start);
            visited.Add(start);

            while (stack.Count > 0)
            {
                int node = stack.Pop();
                result.Add(node);
                foreach (var (neighbor, _) in adj[node])
                {
                    if (visited.Contains(neighbor))
                    {
                        continue;
                    }
                    visited.Add(neighbor);
                    stack.Push(neighbor);
                }
            }
            return result;
        }

        // DFS (recursive) helper
        private void DFSUtil(int node, HashSet<int> visited, List<int> result)
        {
            visited.Add(node);
            result.Add(node);
            foreach (var (neighbor, _) in adj[node])
            {
                if (!visited.Contains(neighbor))
                    DFSUtil(neighbor, visited, result);
            }
        }

        public List<int> DFS_Recursive(int start)
        {
            List<int> result = [];
            HashSet<int> visited = [];
            DFSUtil(start, visited, result);
            return result;
        }

        #endregion


        #region Topological Sort

        // Topological Sort (DFS)
        public List<int> TopologicalSort()
        {
            List<int> result = [];
            HashSet<int> visited = [];

            void Dfs(int node)
            {
                visited.Add(node);
                foreach (var (neighbor, _) in adj[node])
                {
                    if (!visited.Contains(neighbor))
                        Dfs(neighbor);
                }
                result.Add(node);
            }
            foreach (var node in adj.Keys)
            {
                if (!visited.Contains(node))
                    Dfs(node);
            }
            result.Reverse();
            return result;
        }
        #endregion


        #region Paths

        // All paths
        public List<List<int>> FindAllPaths(int start, int end)
        {
            List<List<int>> paths = [];
            Stack<int> path = new();

            void DFS(int node)
            {
                path.Push(node);
                if (node == end)
                {
                    paths.Add(path.Reverse().ToList());
                    path.Pop();
                    return;
                }
                foreach (var (neighbor, _) in adj[node])
                {
                    if (!path.Contains(neighbor))
                        DFS(neighbor);
                }
                path.Pop();
            }
            DFS(start);
            return paths;
        }

        // Count paths
        public int CountPaths(int start, int end)
        {
            int count = 0;
            HashSet<int> path = [];

            void DFS(int node)
            {
                path.Add(node);
                if (node == end)
                {
                    count++;
                }
                else
                {
                    foreach (var (neighbor, _) in adj[node])
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

        public List<int> ShortestPathBFS(int start, int end)
        {
            // unweighted
            var parent = new Dictionary<int, int>();
            var queue = new Queue<int>([start]);
            var visited = new HashSet<int> { start };
            parent[start] = -1;

            while (queue.Count > 0)
            {
                int node = queue.Dequeue();
                if (node == end)
                    break;

                foreach (var (neighbor, _) in adj[node])
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

            var path = new List<int>();
            int curr = end;

            while (curr != -1)
            {
                path.Add(curr);
                curr = parent[curr];
            }
            path.Reverse();
            return path;
        }

        public List<int> Dijkstra(int start, int end)
        {
            var dist = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            var prev = new Dictionary<int, int>();
            var pq = new SortedSet<(int, int)>(
                Comparer<(int, int)>.Create(
                    (a, b) =>
                    {
                        int cmp = a.Item1.CompareTo(b.Item2);
                        return cmp == 0 ? a.Item2.CompareTo(b.Item2) : cmp;
                    }
                )
            );

            dist[start] = 0;
            pq.Add((0, start));

            while (pq.Count > 0)
            {
                var (d, node) = pq.Min;
                pq.Remove(pq.Min);

                if (node == end)
                    break;

                foreach (var (neighbor, weight) in adj[node])
                {
                    int newDist = d + weight;

                    if (newDist < dist[neighbor])
                    {
                        if (dist[neighbor] != int.MaxValue)
                            pq.Remove((dist[neighbor], neighbor));

                        dist[neighbor] = newDist;
                        prev[neighbor] = node;
                        pq.Add((newDist, neighbor));
                    }
                }
            }
            if (!prev.ContainsKey(end))
                return [];

            var path = new List<int>();
            int curr = end;
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

        // Directed Cycle Detection
        public bool HasCycleDirected()
        {
            var visited = new HashSet<int>();
            var stack = new HashSet<int>();

            bool Dfs(int node)
            {
                if (stack.Contains(node))
                    return true;
                if (visited.Contains(node))
                    return false;

                visited.Add(node);
                stack.Add(node);
                foreach (var (neighbor, _) in adj[node])
                {
                    if (Dfs(neighbor))
                        return true;
                }
                stack.Remove(node);
                return false;
            }
            foreach (var node in adj.Keys)
            {
                if (Dfs(node))
                    return true;
            }
            return false;
        }

        // Find Directed Cycle
        public List<int> FindCycle()
        {
            var parent = new Dictionary<int, int>();
            var stack = new HashSet<int>();

            List<int> cycle = [];

            bool Dfs(int node)
            {
                stack.Add(node);
                foreach (var (neighbor, _) in adj[node])
                {
                    if (!parent.ContainsKey(neighbor))
                    {
                        parent[neighbor] = node;
                        if (Dfs(neighbor))
                            return true;
                    }
                    else if (stack.Contains(neighbor))
                    {
                        // backtrack to find cycle path
                        int current = node;
                        cycle.Add(neighbor);
                        while (current != neighbor)
                        {
                            cycle.Add(current);
                            current = parent[current];
                        }
                        cycle.Reverse();
                        return true;
                    }
                }
                stack.Remove(node);
                return false;
            }
            foreach (var node in adj.Keys)
            {
                if (!parent.ContainsKey(node))
                {
                    parent[node] = -1;
                    if (Dfs(node))
                        return cycle;
                }
            }
            return cycle;
        }

        #endregion


        #region Connectivity

        public bool IsConnected()
        {
            if (adj.Keys.Count == 0)
                return true;
            var start = adj.Keys.First();
            var reached = BFS(start);
            return reached.Count == adj.Keys.Count;
        }

        public List<List<int>> COnnectedComponents()
        {
            var components = new List<List<int>>();
            var visited = new HashSet<int>();

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
            return COnnectedComponents().Count;
        }

        #endregion


        #region Graph Coloring

        public int[] GraphColoring(int numColors)
        {
            int[] color = new int[adj.Keys.Max() + 1];
            Array.Fill(color, -1);

            foreach (var node in adj.Keys)
            {
                var unavailable = new HashSet<int>();

                foreach (var (neighbor, _) in adj[node])
                {
                    if (color[neighbor] != -1)
                        unavailable.Add(color[neighbor]);
                }
                for (int c = 0; c < numColors; c++)
                {
                    if (!unavailable.Contains(c))
                    {
                        color[node] = c;
                        break;
                    }
                }
                if (color[node] == -1)
                    throw new Exception("Graph cannot be coloring with given number of colors.");
            }
            return color;
        }

        #endregion


        #region AStar

        public List<int> AStar(int start, int end, Func<int, int, int>? heuristic = null)
        {
            heuristic ??= (a, b) => 0;
            var gScore = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            gScore[start] = 0;

            var fScore = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            fScore[start] = heuristic(start, end);

            var cameFrom = new Dictionary<int, int>();

            var open = new SortedSet<(int, int)>(
                Comparer<(int, int)>.Create(
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
                if (current == end)
                    break;

                open.Remove(open.Min);

                foreach (var (neighbor, weight) in adj[current])
                {
                    int tentativeG = gScore[current] + weight;

                    if (tentativeG < gScore[neighbor])
                    {
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + heuristic(neighbor, end);
                        cameFrom[neighbor] = current;

                        if (!open.Contains((fScore[neighbor], neighbor)))
                            open.Add((fScore[neighbor], neighbor));
                    }
                }
            }
            if (!cameFrom.ContainsKey(end))
                return [];

            var path = new List<int>();
            int curr = end;

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

        public List<int> BellmanFord(int start, int end)
        {
            var dist = adj.Keys.ToDictionary(k => k, k => int.MaxValue);
            var pred = new Dictionary<int, int>();

            dist[start] = 0;

            for (int i = 0; i < adj.Keys.Count - 1; i++)
            {
                foreach (var u in adj.Keys)
                {
                    if (dist[u] == int.MaxValue)
                        continue;

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
                    continue;
                foreach (var (v, w) in adj[u])
                {
                    if (dist[u] + w < dist[v])
                        throw new Exception("Graph contains a negative-weight cycle.");
                }
            }
            if (!pred.ContainsKey(end))
                return [];

            var path = new List<int>();
            int curr = end;
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

        // Kruskal MST
        public List<(int, int, int)> KruskalMST()
        {
            var edges = adj.SelectMany(kvp => kvp.Value.Select(e => (kvp.Key, e.Item1, e.Item2)))
                .ToList();
            edges.Sort((a, b) => a.Item3.CompareTo(b.Item3));

            var parent = adj.Keys.ToDictionary(k => k, k => k);
            int Find(int x)
            {
                while (parent[x] != x)
                    x = parent[x];
                return x;
            }
            void Union(int a, int b)
            {
                int rootA = Find(a);
                int rootB = Find(b);
                if (rootA != rootB)
                    parent[rootB] = rootA;
            }

            List<(int, int, int)> mst = [];

            foreach (var (u, v, w) in edges)
            {
                if (Find(u) != Find(v))
                {
                    mst.Add((u, v, w));
                    Union(u, v);
                }
            }
            return mst;
        }

        // Prim MST
        public List<(int, int, int)> PrimMST()
        {
            if (adj.Count == 0)
                return [];

            var inMST = new HashSet<int>();
            var pq = new SortedSet<(int, int, int)>(
                Comparer<(int, int, int)>.Create(
                    (a, b) =>
                    {
                        int cmp = a.Item3.CompareTo(b.Item3);
                        if (cmp == 0)
                            cmp = a.Item1.CompareTo(b.Item1);
                        if (cmp == 0)
                            cmp = a.Item2.CompareTo(b.Item2);
                        return cmp;
                    }
                )
            );

            int start = adj.Keys.First();
            inMST.Add(start);
            foreach (var (neighbor, weight) in adj[start])
                pq.Add((start, neighbor, weight));

            List<(int, int, int)> mst = [];

            while (pq.Count > 0 && inMST.Count < adj.Keys.Count)
            {
                var (u, v, w) = pq.Min;
                pq.Remove(pq.Min);
                if (inMST.Contains(v))
                    continue;

                inMST.Add(v);
                mst.Add((u, v, w));

                foreach (var (neighbor, weight) in adj[v])
                {
                    if (!inMST.Contains(neighbor))
                        pq.Add((v, neighbor, weight));
                }
            }
            return mst;
        }
        #endregion
    }
}
