namespace Modules.DataStructureModules
{
    /// <summary>
    /// Concepts
    ///     - Fixed sized: declared with `new`, memory allocated contiguously.
    ///     - Index-based access: O(1) read/write.
    ///     - Insert/delete: O(n) worse-case due to the shifting elements.
    ///     - Multidimensional arrays: `[,]` vs jagged arrays `[][]`.
    /// Key Practices
    ///     - Rotate array left/right (with/without extra space).
    ///     - Find max/min element
    ///     - Find element using linear/binary search
    ///     - Merge two sorted arrays into one
    /// </summary>

    public static class Arrays
    {
        private static void Reverse(int[] arr, int start, int end)
        {
            while (start < end)
            {
                (arr[start], arr[end]) = (arr[end], arr[start]);
                start++; end--;
            }
        }
        // Rotate array to the right by k positions (using reversal algorithm)
        public static void RotateRight(int[] arr, int k)
        {
            if (arr == null || arr.Length == 0) return;
            int n = arr.Length;
            k %= n;
            Reverse(arr, 0, n-1);
            Reverse(arr, 0, k-1);
            Reverse(arr, k, n-1);
        }

        // Rotate array to the left by k positions
        public static void RotateLeft(int[] arr, int k)
        {
            if (arr == null || arr.Length == 0) return;
            int n = arr.Length;
            k %= n;
            Reverse(arr, 0, k-1);
            Reverse(arr, k, n-1);
            Reverse(arr, 0, n-1);
        }

        // Find maximum element in array
        public static int FindMax(int[] arr)
        {
            if (arr == null || arr.Length == 0)
                throw new ArgumentException("Array cannot be null or empty.");

            int max = arr[0];
            for (int i = 1; i < arr.Length; i++)
                if (arr[i] > max)
                    max = arr[i];
            return max;
        }

        // Find minimum element in array
        public static int FindMin(int[] arr)
        {
            if (arr == null || arr.Length == 0)
                throw new ArgumentException("Array cannot be null or empty.");

            int min = arr[0];
            for (int i = 1; i < arr.Length; i++)
                if (arr[i] < min)
                    if (arr[i] < min)
                        min = arr[i];

            return min;
        }

        // Linear search
        public static int LinearSearch(int[] arr, int target)
        {
            if (arr == null) return -1;
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == target)
                    return i;
            return -1;
        }

        // Binary search (array must be sorted)
        public static int BinarySearch(int[] arr, int target)
        {
            if (arr == null) return -1;

            int left = 0, right = arr.Length - 1;
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (arr[mid] == target) return mid;
                else if (arr[mid] < target) left = mid + 1;
                else right = mid - 1;
            }
            return -1;
        }

        // Merge two sorted arrays
        public static int[] MergeSortedArrays(int[] a, int[] b)
        {
            if (a == null || a.Length == 0 || b == null || b.Length == 0)
                throw new ArgumentException("Input arrays cannot be null.");

            int i = 0, j = 0, k = 0;
            int [] result = new int[a.Length + b.Length];

            while (i < a.Length && j < b.Length)
            {
                if (a[i] <= b[j]) result [k++] = a[i++];
                else result [k++] = b[j++];
            }

            while (i < a.Length) result[k++] = a[i++];
            while (j < b.Length) result[k++] = b[j++];

            return result;
        }
    }
}
