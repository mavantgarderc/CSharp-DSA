namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts
    ///     - Generic collections and the System.Collections.Generic namespace
    ///     - List<T> instantiation, manipulation, and iteration
    ///     - Functional programming with lambda expressions
    ///     - Data querying with LINQ
    /// Key Practices
    ///     - Avoiding index out-of-range errors
    ///     - Using AddRange and RemoveAt responsibly
    ///     - Leveraging FindAll and Contains for clean search logic
    ///     - Combining Sort, Reverse, and LINQ for readable data transformations
    /// </summary>

    public class ListT
    {
        // Add element to list
        public static void Add<T>(List<T> list, T item)
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            list.Add(item);
        }

        // Insert element at index
        public static void InsertAt<T>(List<T> list, int index, T item)
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            if (index < 0 || index > list.Count) throw new ArgumentException(nameof(index));
            list.Insert(index, item);
        }

        // Remove element at index
        public static void RemoveAt<T>(List<T> list, int index)
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            if (index < 0 || index >= list.Count) throw new ArgumentException(nameof(index));
            list.RemoveAt(index);
        }

        // Find index of an item (Linear Search)
        public static int IndexOf<T>(List<T> list, T item)
        {
            if (list == null) return -1;
            return list.IndexOf(item);
        }

        // Sort list in ascending order (requires IComparable)
        public static void SortAscending<T>(List<T> list) where T : IComparable<T>
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            list.Sort();
        }

        // Sort list in descending order (require IComparable)
        public static void SortDescending<T>(List<T> list) where T : IComparable<T>
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            list.Sort((a, b) => b.CompareTo(a));
        }

        // Reverse the list
        public static void Reverse<T>(List<T> list)
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            list.Reverse();
        }

        // Clone a list (deep copy for value types or reference re-assignment)
        public static List<T> Clone<T>(List<T> list)
        {
            if (list == null) throw new ArgumentException("List cannot be null.");
            return new List<T>(list);
        }

        // Merge two lists into a new list
        public static List<T> Merge<T>(List<T> a, List<T> b)
        {
            if (a == null || b == null) throw new ArgumentException("Input lists cannot be null.");
            var result = new List<T>(a.Count + b.Count);
            result.AddRange(a);
            result.AddRange(b);
            return result;
        }
    }
}
