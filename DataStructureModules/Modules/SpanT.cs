namespace DataStructureModules.Modules
{
    /// <summary>
    /// Concepts
    ///     - Stack-only memory access
    ///     - Memory safety without GC allocations
    ///     - Slicing data without copying
    ///     - Lightweight memory views
    /// Key Practices
    ///     - Use Span<T> for in-place, high-performance data operations
    ///     - Avoid heap allocations for transient data
    ///     - Prefer stackalloc when size is small and known
    ///     - Span<T> can’t escape the method scope
    /// </summary>

    public class SpanT
    {
        // Reverses the contents of a span in-place
        public static void ReverseInPlace(Span<int> span)
        {
            for (int i = 0, j = span.Length - 1; i < j; i++, j--)
            {
                (span[i], span[j]) = (span[j], span[i]);
            }
        }

        // Calculates the sum of all elements in the span
        public static int Sum(Span<int> span)
        {
            int sum = 0;
            foreach (var num in span)
                sum += num;
            return sum;
        }

        // Copies elements from the source span to the destination span
        // throws if the destination is too small...
        public static void CopySlice(Span<int> source, Span<int> destination)
        {
            if (!source.TryCopyTo(destination))
                throw new ArgumentException("Destination span is too small.");

            source.CopyTo(destination);
        }

        // Returns a slice of one or two middle elements from the span
        public static Span<int> SliceMiddle(Span<int> span)
        {
            if (span.IsEmpty)
                throw new ArgumentOutOfRangeException(nameof(span), "Span cannot be empty.");

            int start, length;
            if (span.Length % 2 == 0)
            {
                // Even length: return two middle elements
                start = (span.Length / 2) - 1;
                length = 2;
            }
            else
            {
                // Odd length: return single middle element
                start = span.Length / 2;
                length = 1;
            }

            return span.Slice(start, length);
        }

        // Fills the span with a specified value
        public static void Fill(Span<int> span, int value)
        {
            span.Fill(value);
        }

        // Allocates a span on the stack wiuth the given size
        // Only valid for small sizes due to stack constarints...
        public static void WithStackAlloc(int size, Action<Span<int>> action)
        {
            if (size <= 0 || size > 1024)
                throw new ArgumentOutOfRangeException(nameof(size),
                        "Size must be between 1 & 1024.");

            Span<int> buffer = stackalloc int[size];
            action(buffer);
        }
    }
}
