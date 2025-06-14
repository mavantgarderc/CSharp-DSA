namespace Modules.Algorithms
{
    /// <summary>
    /// - Key Practices:
    ///   - implementing extension methods to enhance `IEnumerable` functionality
    ///   - providing generic algorithms to maximize code reuse
    ///   - designing linear search methods with O(n) complexity in mind
    ///   - staying type-safe by employing `Func<T, bool>` predicates
    /// - Concepts:
    ///   - generics (`T`) for reusable methods across different collection types
    ///   - extension methods for extending functionality without modifying original code
    ///   - higher-order functions (`Func`) for flexible match criteria
    ///   - algorithm complexity (linear search) and its O(n) performance characteristics
    /// </summary>

    public static class LinearSearch
    {
        // returns index of first item matching the condition, else -1
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            int idx = 0;
            foreach (var item in collection)
            {
                if (match(item)) return idx;
                idx++;
            }
            return -1;
        }

        // check existence of something in something
        public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> match)
            => collection.IndexOf(match) != -1;

        // list of matching items
        public static List<T> AllMatches<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (match(item)) result.Add(item);
            }
            return result;
        }

        // counts the times condition is true
        public static int CountMatches<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            int count = 0;
            foreach (var item in collection)
            {
                if (match(item)) count++;
            }
            return count;
        }

        // returns the first matched item, else none
        public static T? FirstOrDefaultCustom<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (match(item)) return item;
            }
            return default;
        }

        // gives the lst index matching the condition or -1 if not found
        public static int LastIndexOf<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            int idx = -1;
            int i = 0;
            foreach (var item in collection)
            {
                if (match(item)) idx = i;
                i++;
            }
            return idx;
        }

        // same as `AllMatches`
        public static List<T> FindAll<T>(this IEnumerable<T> collection, Func<T, bool> match)
            => collection.AllMatches(match);

        // if all elements satify the condition
        public static bool AllMatchesCondition<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (!match(item)) return false;
            }
            return true;
        }

        // if any element meets condition
        public static bool AnyMatch<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (match(item)) return true;
            }
            return false;
        }

        // removes all mattchging elements, returns the count removed
        public static int RemoveAll<T>(this List<T> collection, Func<T, bool> match)
        {
            int removed = 0;
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (match(collection[i]))
                {
                    collection.RemoveAt(i);
                    removed++;
                }
            }
            return removed;
        }

        // returns single mathch, else fallback if zero/ multiple match
        public static T? SingleOrDefaultCustom<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            T? matchItem = default;
            int count = 0;
            foreach (var item in collection)
            {
                if (match(item))
                {
                    matchItem = item;
                    count++;
                    if (count > 1) return default;
                }
            }
            return matchItem;
        }
    }
}
