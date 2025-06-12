namespace Modules.DataStructures
{
    public class SequentialTraversal
    {
        public void TraverseArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(array[i]);
            }
        }

        public void TraverseList(List<int> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        public void TraverseLinkedList(LinkedList<int> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        public void Traverse2DArray(int[,] matrix)
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

        public void TraverseJaggedArray(int[][] jagged)
        {
            foreach(var row in jagged)
            {
                foreach(var item in row)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public void TraverseWithIndex<T>(IEnumerable<T> collection)
        {
            int i = 0;

            foreach (var item in collection)
            {
                Console.WriteLine($"[{i++}] = {item}");
            }
        }

        public void TravereReverse<T>(IList<T> list)
        {
            for (int i = list.Count-1; i >= 0; i--)
            {
                Console.WriteLine(list[i]);
            }
        }

        public void TraverseStack<T>(Stack<T> stack)
        {
            foreach (var item in stack)
            {
                Console.WriteLine(item);
            }
        }

        public void TraverseQueue<T>(Queue<T> queue)
        {
            foreach (var item in queue)
            {
                Console.WriteLine(item);
            }
        }

        public void TraverseDictionary<K, V>(Dictionary<K, V> dict)
            where K : notnull
        {
            foreach (var kv in dict)
            {
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }
        }

        public void TraverseSet<T>(HashSet<T> set)
        {
            foreach (var item in set)
            {
                Console.WriteLine(item);
            }
        }

        public void TraverseSpan(Span<int> span)
        {
            for (int i = 0; i < span.Length; i++)
            {
                Console.WriteLine(span[i]);
            }
        }

        public void TraverseString(string s)
        {
            foreach (char c in s)
            {
                Console.WriteLine(c);
            }
        }

        public void TraverseEnumerator<T>(IEnumerable<T> collection)
        {
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current);
            }
        }

        public void TraverseParallel<T>(IEnumerable<T> collection)
        {
            System.Threading.Tasks.Parallel.ForEach(collection, item =>
            {
                Console.WriteLine(item);
            });
        }

        public static void TraversCircularBuffer<T>(T[] buffer, int head, int size)
        {
            int capacity = buffer.Length;

            for (int i = 0; i < size; i++)
            {
                int index = (head+1) % capacity;
                Console.WriteLine(buffer[index]);
            }
        }
    }
}
