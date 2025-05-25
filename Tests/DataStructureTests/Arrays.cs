using Xunit;
using Modules.DataStructureModules;


namespace Tests
{
    public class ArraysTests
    {
        [Fact]
        public void RotateRight_By3_ReturnsCorrectResult()
        {
            int[] input = { 1, 2, 3, 4, 5 };
            Arrays.RotateRight(input, 3);
            Assert.Equal(new[] { 3, 4, 5, 1, 2 }, input);
        }

        [Fact]
        public void RotateLeft_By2_ReturnsCorrectResult()
        {
            int[] input = { 1, 2, 3, 4, 5 };
            Arrays.RotateLeft(input, 2);
            Assert.Equal(new[] { 3, 4, 5, 1, 2 }, input);
        }

        [Fact]
        public void FindMax_ReturnsCorrectValue()
        {
            int[] input = { 4, 7, 2, 9, 5 };
            int max = Arrays.FindMax(input);
            Assert.Equal(9, max);
        }

        [Fact]
        public void FindMin_ReturnCorrectValue()
        {
            int[] input = { 4, 7, 2, 9, 5 };
            int min = Arrays.FindMin(input);
            Assert.Equal(2, min);
        }

        [Fact]
        public void LinearSearch_FindsElement()
        {
            int[] input = { 10, 20, 30, 40 };
            int index = Arrays.LinearSearch(input, 30);
            Assert.Equal(2, index);
        }

        [Fact]
        public void BinarySearch_FindsElement()
        {
            int[] input = { 1, 3, 5, 7, 9 };
            int index = Arrays.BinarySearch(input, 7);
            Assert.Equal(3, index);
        }

        [Fact]
        public void BinarySearch_ElementNotFound_ReturnsMinus1()
        {
            int[] input = { 1, 2, 4, 6 };
            int index = Arrays.BinarySearch(input, 3);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void MergeSortedArrays_ReturnsMergedSortedArray()
        {
            int[] a = { 1, 3, 5 };
            int[] b = { 2, 4, 6 };
            var result = Arrays.MergeSortedArrays(a, b);
            Assert.Equal(new[] { 1, 2, 3, 4, 5, 6 }, result);
        }

        [Fact]
        public void RotateRight_NullInput_DoesNotThrow()
        {
            int[] input = {};
            var exception = Record.Exception(() => Arrays.RotateRight(input, 2));
            Assert.Null(exception);
        }

        [Fact]
        public void BinarySearch_NullInput_ReturnsMinus1()
        {
            int[] input = {};
            int index = Arrays.BinarySearch(input, 5);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void FindMax_EmptyArray_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Arrays.FindMax(Array.Empty<int>()));
        }

        [Fact]
        public void MergeSortedArrays_NullInput_ThrowsException()
        {
            int[] a = {};
            int[] b = { 1, 2, 3 };
            Assert.Throws<ArgumentException>(() => Arrays.MergeSortedArrays(a, b));
        }
    }
}
