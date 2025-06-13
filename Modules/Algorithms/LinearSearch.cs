using System;
using System.Collections.Generic;

namespace Modules.Algorithms
{
    public static class LinearSearch
    {
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

        public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> match)
            => collection.IndexOf(match) != -1;

        public static List<T> AllMatches<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            var result = new List<T>();
            foreach (var item in collection)
            {
                if (match(item)) result.Add(item);
            }
            return result;
        }

        public static int CountMatches<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            int count = 0;
            foreach (var item in collection)
            {
                if (match(item)) count++;
            }
            return count;
        }

        public static T? FirstOrDefaultCustom<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (match(item)) return item;
            }
            return default;
        }

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

        public static List<T> FindAll<T>(this IEnumerable<T> collection, Func<T, bool> match)
            => collection.AllMatches(match);

        public static bool AllMatchesCondition<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (!match(item)) return false;
            }
            return true;
        }

        public static bool AnyMatch<T>(this IEnumerable<T> collection, Func<T, bool> match)
        {
            foreach (var item in collection)
            {
                if (match(item)) return true;
            }
            return false;
        }

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