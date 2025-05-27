using Modules.DataStructures;

namespace Tests
{
    public class SnapTTests
    {
        [Fact]
        public void ReverseInPlace_ReversesSpanCorrectly()
        {
            Span<int> span = stackalloc int[] { 1, 2, 3, 4 };
            SpanT.ReverseInPlace(span);
            Assert.Equal(new int[] { 4, 3, 2, 1 }, span.ToArray());
        }

        [Fact]
        public void Sum_ReturnsCorrectSum()
        {
            Span<int> span = stackalloc int[] { 10, 20, 30 };
            int result = SpanT.Sum(span);
            Assert.Equal(60, result);
        }

        [Fact]
        public void CopySlice_CopiesSuccessfully()
        {
            Span<int> source = stackalloc int[] { 1, 2, 3 };
            Span<int> destination = stackalloc int[3];
            SpanT.CopySlice(source, destination);
            Assert.Equal(source.ToArray(), destination.ToArray());
        }


        [Fact]
        public void CopySlice_ThrowsIfDestinationTooSmall()
        {
            Span<int> a = stackalloc int[] { 1, 2, 3 };
            Span<int> b = stackalloc int[2];

            ArgumentException ex = null!;
            try
            {
                SpanT.CopySlice(a, b);
            }
            catch (ArgumentException e)
            {
                ex = e;
            }

            Assert.NotNull(ex);
            Assert.Contains("Destination span is too small.", ex.Message);
        }


        [Theory]
        [InlineData(new int[] { 1 }, new int[] { 1 })]         // Odd length
            [InlineData(new int[] { 1, 2 }, new int[] { 1, 2 })]   // Even length
                [InlineData(new int[] { 1, 2, 3 }, new int[] { 2 })]   // Odd length
                    [InlineData(new int[] { 1, 2, 3, 4 }, new int[] { 2, 3 })] // Even length
                        public void SliceMiddle_ReturnsCorrectMiddleElements(int[] input, int[] expected)
                        {
                            Span<int> span = stackalloc int[input.Length];
                            input.CopyTo(span);
                            Span<int> middle = SpanT.SliceMiddle(span);
                            Assert.Equal(expected, middle.ToArray());
                        }

        [Theory]
        [InlineData(1)]
        [InlineData(128)]
        public void StackAllocate_AllocatesCorrectSize(int size)
        {
            SpanT.WithStackAlloc(size, span =>
                    Assert.Equal(size, span.Length)
                    );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2048)]
        public void StackAllocate_ThrowsInvalidSize(int size)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                    {
                    SpanT.WithStackAlloc(size, span => { /* no-op */});
                    });
        }
    }
}
