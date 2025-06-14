namespace Modules.Algorithms
{
    /// <summary> 
    /// Key practices:
    ///      - Use extension methods for fluent, chainable stream processing
    ///      - Cover both synchronous and asynchronous data flows
    ///      - Ensure methods are allocation-conscious for high-throughput scenarios
    ///      - Favor composability and separation of concerns in algorithmic pipelines
    /// Concepts:
    ///      - Functional transformations (Map, Filter, Reduce, etc.)
    ///      - Batching and grouping (Window, Chunk, Pairwise)
    ///      - Uniqueness and distinctness (DistinctBy)
    ///      - Early termination and control flow (TakeUntil, Skip, Take, ForEach)
    /// </summary>

    public static class StreamProcessing
    {

        #region Synchronous Variants
        // synchro map (Select)
        public static IEnumerable<TResult> Map<T, TResult>(
                this IEnumerable<T> source,
                Func<T, TResult> selector)
        {
            foreach (var item in source)
            {
                yield return selector(item);
            }
        }

        // synchro filter (Where)
        public static IEnumerable<T> Filter<T>(
                this IEnumerable<T> source,
                Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        // synchro window batching
        public static IEnumerable<List<T>> Window<T>(
                this IEnumerable<T> source,
                int windowsize)
        {
            if (windowsize <= 0)
                throw new ArgumentOutOfRangeException(nameof(windowsize));

            var buffer = new List<T>(windowsize);

            foreach(var iten in source)
            {
                yield return new List<T>(buffer);
                buffer.Clear();
            }
            if (buffer.Count > 0)
            {
                yield return buffer;
            }
        }

        // synchro takeutil (yields until predicatei s true)
        public static IEnumerable<T> TakeUtil<T>(
                this IEnumerable<T> source,
                Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                yield return item;

                if (predicate(item))
                {
                    yield break;
                }
            }
        }

        // synchro aggregate/reduce
        public static TAccumulate Reduce<T, TAccumulate>(
                this IEnumerable<T> source,
                TAccumulate seed,
                Func<TAccumulate, T, TAccumulate> func)
        {
            var acc = seed;

            foreach (var item in source)
            {
                acc = func(acc, item);
            }
            return acc;
        }

        // synchro DistinctBy
        public static IEnumerable<T> DistinctBy<T, TKey>(
                this IEnumerable<T> source,
                Func<T, TKey> keySelector)
        {
            var seen = new HashSet<TKey>();

            foreach (var item in source)
            {
                if (seen.Add(keySelector(item)))
                    yield return item;
            }
        }

        // synchro Skip
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int count)
        {
            int i = 0;

            foreach (var item in source)
            {
                if (i++ >= count)
                {
                    yield return item;
                }
            }
        }

        // synchro Take
        public static IEnumerable<T> Take<T>(this IEnumerable<T> source, int count)
        {
            int i = 0;

            foreach (var item in source)
            {
                if (i++ >= count) yield break;

                yield return item;
            }
        }

        // Synchro Pairwise
        public static IEnumerable<(T, T)> Pairwise<T>(this IEnumerable<T> source)
        {
            using var e = source.GetEnumerator();

            if (!e.MoveNext()) yield break;

            var prev = e.Current;
            while (e.MoveNext())
            {
                yield return (prev, e.Current);
                prev = e.Current;
            }
        }

        // Synchro Chunk
        public static IEnumerable<List<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkSize));

            var buffer = new List<T>(chunkSize);

            foreach (var item in source)
            {
                buffer.Add(item);

                if (buffer.Count == chunkSize)
                {
                    yield return new List<T>(buffer);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                yield return buffer;
        }

        // synchro ForEach (side effect)
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }
        #endregion

        #region Asynchronous Variants
        #endregion
        // async Map (Select)
        public static async IAsyncEnumerable<TResult> MapAsync<T, TResult>(
                this IAsyncEnumerable<T> source,
                Func<T, TResult> selector,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
                yield return selector(item);
        }

        // async filter
        public static async IAsyncEnumerable<T> FilterAsync<T>(
                this IAsyncEnumerable<T> source,
                Func<T, bool> predicate,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
                if (predicate(item))
                    yield return item;
        }

        // async windowed batching
        public static async IAsyncEnumerable<List<T>> WIndowAsync<T>(
                this IAsyncEnumerable<T> source,
                int windowSize,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (windowSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(windowSize));

            var buffer = new List<T>(windowSize);

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                buffer.Add(item);

                if (buffer.Count == windowSize)
                {
                    yield return new List<T>(buffer);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                yield return buffer;
        }

        // async TakeUntil
        public static async IAsyncEnumerable<T> TakeUntilAsync<T>(
                this IAsyncEnumerable<T> source,
                Func<T, bool> predicate,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                yield return item;

                if (predicate(item))
                    break;
            }
        }

        // async reduce/aggregate
        public static async Task<TAccumulate> ReduceAsync<T, TAccumulate>(
                this IAsyncEnumerable<T> source,
                TAccumulate seed,
                Func<TAccumulate, T, TAccumulate> func,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var acc = seed;
            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                acc = func(acc, item);
            }
            return acc;
        }

        // async DistinctBy
        public static async IAsyncEnumerable<T> DistinctBy<T, TKey>(
                this IAsyncEnumerable<T> source,
                Func<T, TKey> keySelector,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var seen = new HashSet<TKey>();

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (seen.Add(keySelector(item)))
                {
                    yield return item;
                }
            }
        }

        // async Skip
        public static async IAsyncEnumerable<T> SkipAsync<T>(
                this IAsyncEnumerable<T> source,
                int count,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int i = 0;

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (i++ >= count)
                {
                    yield return item;
                }
            }
        }

        // async Take
        public static async IAsyncEnumerable<T> TakeAsync<T>(
                this IAsyncEnumerable<T> source,
                int count,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int i = 0;

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                if (i++ >= count) yield break;
                yield return item;
            }
        }

        // async Pairwise
        public static async IAsyncEnumerable<(T, T)> PairwiseAsync<T>(
                this IAsyncEnumerable<T> source,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var enumerator = source.GetAsyncEnumerator(cancellationToken);

            try
            {
                if (!await enumerator.MoveNextAsync()) yield break;

                var prev = enumerator.Current;
                while (await enumerator.MoveNextAsync())
                {
                    yield return (prev, enumerator.Current);
                    prev = enumerator.Current;
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
        }

        // async Chunk
        public static async IAsyncEnumerable<List<T>> ChunkAsync<T>(
                this IAsyncEnumerable<T> source,
                int chunkSize,
                [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (chunkSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(chunkSize));

            var buffer = new List<T>(chunkSize);

            await foreach (var item in source.WithCancellation(cancellationToken))
            {
                buffer.Add(item);

                if (buffer.Count == chunkSize)
                {
                    yield return new List<T>(buffer);
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
                yield return buffer;
        }

        // async ForEach
        public static async Task ForEachAsync<T>(
                this IAsyncEnumerable<T> source,
                Action<T> action,
                CancellationToken cancellationToken = default)
        {
            await foreach (var item in source.WithCancellation(cancellationToken))
                action(item);
        }
    }
}
