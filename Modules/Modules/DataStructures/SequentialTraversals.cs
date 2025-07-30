namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - Sequential and parallel traversal of common .NET collections
    ///     - Generic programming for type flexibility
    ///     - Iteration over linear and nonlinear data structures (arrays, stacks, queues, linked lists, dictionaries, sets)
    ///     - Handling specialized structures (jagged arrays, 2D arrays, spans, circular buffers)
    /// Key Practices:
    ///     - Using appropriate iteration patterns for each collection type
    ///     - Leveraging interfaces like IEnumerable<T> and IList<T> for abstraction
    ///     - Ensuring forward and reverse traversal capabilities
    ///     - Demonstrating safe and efficient use of enumerators and parallelism
    /// </summary>
    
    public class NewBaseType
    {
        public static void TraverseStack<T>(Stack<T> stack)
        {
            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }
        }
    }

    public class SequentialTraversal : NewBaseType
    {
        public static void TraverseArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(array[i]);
            }
        }

        public static void TraverseList(List<int> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        public static void TraverseLinkedList(LinkedList<int> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        public static void Traverse2DArray(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.WriteLine(matrix[i,j]);
                }
            }
        }

        public static void TraverseJaggedArray(int[][] jagged)
        {
            foreach(var row in jagged)
            {
                foreach(var item in row)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void TraverseWithIndex<T>(IEnumerable<T> collection)
        {
            int i = 0;

            foreach (var item in collection)
            {
                Console.WriteLine($"[{i++}] = {item}");
            }
        }

        public static void TravereReverse<T>(IList<T> list)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                Console.WriteLine(list[i]);
            }
        }

        public static void TraverseQueue<T>(Queue<T> queue)
        {
            foreach (var item in queue)
            {
                Console.WriteLine(item);
            }
        }

        public static void TraverseDictionary<K, V>(Dictionary<K, V> dict)
            where K : notnull
        {
            foreach (var kv in dict)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
        }

        public static void TraverseSet<T>(HashSet<T> set)
        {
            foreach (var item in set)
            {
                Console.WriteLine(item);
            }
        }

        public static void TraverseSpan(Span<int> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                Console.WriteLine(span[i]);
            }
        }

        public static void TraverseString(string s)
        {
            foreach (char c in s)
            {
                Console.WriteLine(c);
            }
        }

        public static void TraverseEnumerator<T>(IEnumerable<T> collection)
        {
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current);
            }
        }

        public static void TraverseParallel<T>(IEnumerable<T> collection)
        {
            System.Threading.Tasks.Parallel.ForEach(collection, item =>
            {
                Console.WriteLine(item);
            });
        }

        public static void TraverseCircularBuffer<T>(T[] buffer, int head, int size)
        {
            int capacity = buffer.Length;

            for (int i = 0; i < size; i++)
            {
                int index = (head + 1 + i) % capacity;
                Console.WriteLine(buffer[index]);
            }
        }
    }
}
