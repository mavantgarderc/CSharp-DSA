using static Modules.DataStructures.Arrays;

namespace Tests.DataStructureTests
{
    public class ArraysTests
    {
        private static readonly int[] expectedArray_1234 = [1, 2, 3, 4];
        private static readonly int[] expectedArray_34512 = [3, 4, 5, 1, 2];
        private static readonly int[] expectedArray_45123 = [4, 5, 1, 2, 3];
        private static readonly int[] expectedArray_123456 = [1, 2, 3, 4, 5, 6];
        private static readonly string[] expectedArray_cba = ["c", "b", "a"];

        [Fact]
        public void RotateRight_By3_ReturnsCorrectResult() {
            int[] input = [1, 2, 3, 4, 5];
            RotateRight(input, 3);
            Assert.Equal(expectedArray_34512, input);
        }
        [Fact]
        public void RotateLeft_By2_ReturnsCorrectResult() {
            int[] input = [1, 2, 3, 4, 5];
            RotateLeft(input, 2);
            Assert.Equal(expectedArray_34512, input);
        }
        [Fact]
        public void FindMax_ReturnsCorrectValue() {
            int[] input = [4, 7, 2, 9, 5];
            int max = FindMax(input);
            Assert.Equal(9, max);
        }
        [Fact]
        public void FindMin_ReturnCorrectValue() {
            int[] input = [4, 7, 2, 9, 5];
            int min = FindMin(input);
            Assert.Equal(2, min);
        }
        [Fact]
        public void LinearSearch_FindsElement() {
            int[] input = [10, 20, 30, 40];
            int index = LinearSearch(input, 30);
            Assert.Equal(2, index);
        }
        [Fact]
        public void BinarySearch_FindsElement() {
            int[] input = [1, 3, 5, 7, 9];
            int index = BinarySearch(input, 7);
            Assert.Equal(3, index);
        }
        [Fact]
        public void BinarySearch_ElementNotFound_ReturnsMinus1() {
            int[] input = [1, 2, 4, 6];
            int index = BinarySearch(input, 3);
            Assert.Equal(-1, index);
        }
        [Fact]
        public void MergeSortedArrays_ReturnsMergedSortedArray() {
            int[] a = [1, 3, 5];
            int[] b = [2, 4, 6];
            var result = MergeSortedArrays(a, b);
            Assert.Equal(expectedArray_123456, result);
        }
        [Fact]
        public void RotateRight_NullInput_DoesNotThrow() {
            int[] input = [];
            var exception = Record.Exception(() => RotateRight(input, 2));
            Assert.Null(exception);
        }
        [Fact]
        public void BinarySearch_NullInput_ReturnsMinus1() {
            int[] input = [];
            int index = BinarySearch(input, 5);
            Assert.Equal(-1, index);
        }
        [Fact]
        public void FindMax_EmptyArray_ThrowsException() {
            _ = Assert.Throws<ArgumentException>(() => FindMax([]));
        }
        [Fact]
        public void MergeSortedArrays_NullInput_ThrowsException() {
            int[] a = [];
            int[] b = [1, 2, 3];
            Assert.Throws<ArgumentException>(() => MergeSortedArrays(a, b));
        }
        [Fact]
        public void DynamicArray_AddElements_ShouldIncreaseCount() {
            var arr = new DynamicArray<int>();
            arr.Add(10);
            arr.Add(20);

            Assert.Equal(2, arr.Count);
            Assert.Equal(10, arr[0]);
            Assert.Equal(20, arr[1]);
        }
        [Fact]
        public void Test_RotateRight() {
            int[] arr = [1, 2, 3, 4, 5];
            RotateRight(arr, 2);
            Assert.Equal(expectedArray_45123, arr);
        }

        [Fact]
        public void Test_RotateLeft() {
            int[] arr = [1, 2, 3, 4, 5];
            RotateLeft(arr, 2);
            Assert.Equal(expectedArray_34512, arr);
        }
        [Fact]
        public void Test_FindMax() {
            int[] arr = [1, 9, 3, 7];
            Assert.Equal(9, FindMax(arr));
        }
        [Fact]
        public void Test_FindMin() {
            int[] arr = [2, 3, 0, 5];
            Assert.Equal(0, FindMin(arr));
        }
        [Fact]
        public void Test_LinearSearch() {
            int[] arr = [10, 20, 30];
            Assert.Equal(1, LinearSearch(arr, 20));
            Assert.Equal(-1, LinearSearch(arr, 99));
        }
        [Fact]
        public void Test_BinarySearch() {
            int[] arr = [10, 20, 30, 40];
            Assert.Equal(2, BinarySearch(arr, 30));
            Assert.Equal(-1, BinarySearch(arr, 5));
        }
        [Fact]
        public void Test_MergeSortedArrays() {
            int[] a = [1, 3, 5];
            int[] b = [2, 4, 6];
            Assert.Equal(expectedArray_123456, MergeSortedArrays(a, b));
        }
        [Fact]
        public void Test_Reverse_Generic() {
            string[] arr = ["a", "b", "c"];
            Reverse(arr);
            Assert.Equal(expectedArray_cba, arr);
        }
        [Fact]
        public void Test_Flatten_Generic() {
            int[,] multi = new int[,] { { 1, 2 }, { 3, 4 } };
            Assert.Equal(expectedArray_1234, Flatten(multi));
        }
        [Fact]
        public void Test_ToRectangular_Generic() {
            int[][] jagged = [
                [1, 2],
                [3, 4]
            ];

            var result = ToRectangular(jagged);
            Assert.Equal(1, result[0, 0]);
            Assert.Equal(2, result[0, 1]);
            Assert.Equal(3, result[1, 0]);
            Assert.Equal(4, result[1, 1]);
        }
    }

    public class DynamicArrayTests
    {
        [Fact]
        public void Test_DynamicArray_Add() {
            var da = new DynamicArray<int>();
            da.Add(1);
            da.Add(2);
            Assert.Equal(2, da.Count);
            Assert.Equal(1, da[0]);
            Assert.Equal(2, da[1]);
        }
        [Fact]
        public void Test_DynamicArray_Insert() {
            var da = new DynamicArray<int>();
            da.Add(1);
            da.Add(3);
            da.Insert(1, 2);
            Assert.Equal(3, da.Count);
            Assert.Equal(2, da[1]);
        }
        [Fact]
        public void Test_DynamicArray_RemoveAt() {
            var da = new DynamicArray<int>();
            da.Add(10);
            da.Add(20);
            da.Add(30);
            da.RemoveAt(1);
            Assert.Equal(2, da.Count);
            Assert.Equal(30, da[1]);
        }
        [Fact]
        public void Test_DynamicArray_Clear() {
            var da = new DynamicArray<string>();
            da.Add("hello");
            da.Clear();
            Assert.Equal(0, da.Count);
        }
        [Fact]
        public void Test_DynamicArray_Contains() {
            var da = new DynamicArray<int>();
            da.Add(1);
            da.Add(2);
            Assert.True(da.Contains(1));
            Assert.False(da.Contains(3));
        }

        [Fact]
        public void Test_DynamicArray_IndexOf() {
            var da = new DynamicArray<int>();
            da.Add(10);
            da.Add(20);
            Assert.Equal(1, da.IndexOf(20));
            Assert.Equal(-1, da.IndexOf(30));
        }

        [Fact]
        public void Test_DynamicArray_ToString() {
            var da = new DynamicArray<string>();
            da.Add("a");
            da.Add("b");
            Assert.Equal("[a, b]", da.ToString());
        }
        [Fact]
        public void Test_DynamicArray_IndexerGetSet() {
            var da = new DynamicArray<int>();
            da.Add(5);
            da[0] = 10;
            Assert.Equal(10, da[0]);
        }
        [Fact]
        public void DynamicArray_IndexerOutOfRange_ThrowsException() {
            var da = new DynamicArray<int>();
            da.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>(()=> da.Insert(-1, 5));
            Assert.Throws<ArgumentOutOfRangeException>(() => da.RemoveAt((1)));
        }
        [Fact]
        public void DynamicArray_RemoveAtOutOfRange_ThrowsException() {
            var da = new DynamicArray<int>();
            da.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => da.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => da.RemoveAt(1));
        }
    }
}
