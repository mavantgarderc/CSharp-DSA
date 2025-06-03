namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts:
    ///     - extract tuple elements
    ///     - multiple tuples to nested/flattened
    ///     - nested tuple to flattened
    ///     - safe generic tuple
    /// Key Practices:
    ///     - method chainiing for fluent tupel transformations
    ///     - data integrity
    ///     - functional idioms
    ///     - compile-time checks
    /// </summary>

    public static class TupleUtils
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static (T1, T2) CreateValueTuple<T1, T2>(T1 item1, T2 item2)
        {
            return (item1, item2);
        }

        public static (T1, T2) ToValueTuple<T1, T2>(this Tuple<T1, T2> tuple)
        {
            return (tuple.Item1, tuple.Item2);
        }

        public static Tuple<T1, T2> ToTuple<T1, T2>(this (T1, T2) valueTuple)
        {
            return new Tuple<T1, T2>(valueTuple.Item1, valueTuple.Item2);
        }

        public static (TResult1, TResult2) Map<T1, T2, TResult1, TResult2>(
                this (T1, T2) tuple,
                Func<T1, TResult1> map1,
                Func<T2, TResult2> map2) 
        { 
            return (map1(tuple.Item1), map2(tuple.Item2));
        }

        public static Tuple<TResult1, TResult2> Map<T1, T2, TResult1, TResult2>(
                this Tuple<T1, T2> tuple,
                Func<T1, TResult1> map1,
                Func<T2, TResult2> map2) =>
            new(map1(tuple.Item1), map2(tuple.Item2));

        public static (T1, T2, T3) Flatten<T1, T2, T3>(this ((T1, T2), T3) tuple)
        {
            return (tuple.Item1.Item1, tuple.Item1.Item2, tuple.Item2);
        }

        public static Tuple<T1, T2, T3> Flatten<T1, T2, T3>(this Tuple<Tuple<T1, T2>, T3> tuple)
        {
            return new Tuple<T1, T2, T3>(tuple.Item1.Item1, tuple.Item1.Item2, tuple.Item2);
        }

        public static IEnumerable<(T1, T2)> ZipToValueTuples<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
        {
            return first.Zip(second, (a, b) => (a, b));
        }

        public static IEnumerable<Tuple<T1, T2>> ZipToTuples<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second)
        {
            return first.Zip(second, (a, b) => new Tuple<T1, T2>(a, b));
        }

        public static bool StructuralEquals<T1, T2>((T1, T2) a, (T1, T2) b) =>
            EqualityComparer<T1>.Default.Equals(a.Item1, b.Item1)
            && EqualityComparer<T2>.Default.Equals(a.Item2, b.Item2);

        public static bool StructuralEquals<T1, T2>(Tuple<T1, T2> a, Tuple<T1, T2> b) =>
            EqualityComparer<T1>.Default.Equals(a.Item1, b.Item1)
            && EqualityComparer<T2>.Default.Equals(a.Item2, b.Item2);

        public static (T2, T1) Swap<T1, T2>(this (T1, T2) tuple)
        {
            return (tuple.Item2, tuple.Item1);
        }

        public static Tuple<T2, T1> Swap<T1, T2>(this Tuple<T1, T2> tuple)
        { 
            return new Tuple<T2, T1>(tuple.Item2, tuple.Item1);
        }

        public static void Apply<T1, T2>((T1, T2) tuple, Action<T1, T2> action)
        {
            action(tuple.Item1, tuple.Item2);
        }

        public static TResult Select<T1, T2, TResult>((T1, T2) tuple, Func<T1, T2, TResult> selector)
        {
            return selector(tuple.Item1, tuple.Item2);
        }

        public static (TResult, TResult) TransformAll<T1, TResult>((T1, T1) tuple, Func<T1, TResult> map)
        { 
            return (map(tuple.Item1), map(tuple.Item2));
        }

        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<(T1, T2)> tuples, bool throwIfNull = false)
            where T1 : notnull
        {
            return throwIfNull
                ? tuples.ToDictionary(
                    t => t.Item1,
                    t => t.Item2 ?? throw new InvalidOperationException("Tupble.Item2 is null."))
                : tuples.ToDictionary(
                    t => t.Item1,
                    t => t.Item2!);
        }

        public static (T2, T3, T1) RotateLeft<T1, T2, T3>((T1, T2, T3) tuple)
        {
            return (tuple.Item2, tuple.Item3, tuple.Item1);
        }

        public static (T3, T1, T2) RotateRight<T1, T2, T3>((T1, T2, T3) tuple)
        {
            return (tuple.Item3, tuple.Item1, tuple.Item2);
        }

        public static (T1, T2, T1, T2) Duplicate<T1, T2>((T1, T2) tuple)
        {
            return (tuple.Item1, tuple.Item2, tuple.Item1, tuple.Item2);
        }

        public static bool Contains<T1, T2>(this (T1, T2) tuple, object value)
        {
            return Equals(tuple.Item1, value) || Equals(tuple.Item2, value);
        }

        public static int IndexOf<T1, T2>(this (T1, T2) tuple, object value)
        {
            if (Equals(tuple.Item1, value)) return 0;
            if (Equals(tuple.Item2, value)) return 1;
            return -1;
        }
    }
}
