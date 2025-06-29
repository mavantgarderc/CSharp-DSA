using System.Text;

namespace Modules.DataStructures
{
    /// <summary>
    /// Concepts
    ///     - Fixed sized: declared with `new`, memory allocated contiguously.
    ///     - Index-based access: O(1) read/write.
    ///     - Insert/delete: O(n) worse-case due to the shifting elements.
    ///     - Multidimensional arrays: `[,]` vs jagged arrays `[][]`.
    ///     - Dynamic arrays: auto-resizing linear containers with amortized O(1) appends.
    /// Key Practices
    ///     - Rotate array left/right (with/without extra space).
    ///     - Find max/min element
    ///     - Find element using linear/binary search
    ///     - Merge two sorted arrays into one
    ///     - Flatten 2D rectangular arrays to 1D linear arrays.
    ///     - Reverse arrays in-place (generic).
    ///     - Utilize `DynamicArray<T>`: supports
    ///          Add, Insert, RemoveAt, Clear, Contains,
    ///          IndexOf, indexer access, and string representation.
    /// </summary>
    public class Arrays
    {
        private static void Reverse(int[] arr, int start, int end)
        {
            while (start < end)
            {
                (arr[start], arr[end]) = (arr[end], arr[start]);
                start++;
                end--;
            }
        }

        public static void RotateRight(int[] arr, int k)
        {
            if (arr == null || arr.Length == 0)
            {
                return;
            }

            int n = arr.Length;
            k %= n;
            Reverse(arr, 0, n - 1);
            Reverse(arr, 0, k - 1);
            Reverse(arr, k, n - 1);
        }

        public static void RotateLeft(int[] arr, int k)
        {
            if (arr == null || arr.Length == 0)
            {
                return;
            }
            int n = arr.Length;
            k %= n;

            Reverse(arr, 0, k - 1);
            Reverse(arr, k, n - 1);
            Reverse(arr, 0, n - 1);
        }

        public static int FindMax(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                throw new ArgumentException("Array cannot be null or empty.");
            }

            int max = arr[0];

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] > max)
                {
                    max = arr[i];
                }
            }

            return max;
        }

        public static int FindMin(int[] arr)
        {
            if (arr == null || arr.Length == 0)
            {
                throw new ArgumentException("Array cannot be null or empty.");
            }

            int min = arr[0];

            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] < min)
                {
                    if (arr[i] < min)
                    {
                        min = arr[i];
                    }
                }
            }

            return min;
        }

        public static int LinearSearch(int[] arr, int target)
        {
            if (arr == null)
            {
                return -1;
            }

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == target)
                {
                    return i;
                }
            }

            return -1;
        }

        // array must be sorted
        public static int BinarySearch(int[] arr, int target)
        {
            if (arr == null)
            {
                return -1;
            }

            int left = 0,
                right = arr.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (arr[mid] == target)
                {
                    return mid;
                }
                else if (arr[mid] < target)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }

        public static int[] MergeSortedArrays(int[] a, int[] b)
        {
            if (a == null || a.Length == 0 || b == null || b.Length == 0)
                throw new ArgumentException("input cannot be null.");

            int i = 0,
                j = 0,
                k = 0;
            int[] result = new int[a.Length + b.Length];

            while (i < a.Length && j < b.Length)
            {
                if (a[i] <= b[j]) { result[k++] = a[i++]; }
                else { result[k++] = b[j++]; }
            }

            while (i < a.Length)
            {
                result[k++] = a[i++];
            }
            while (j < b.Length) {
                result[k++] = b[j++];
            }

            return result;
        }

        public static void Reverse<T>(T[] array)
        {
            ArgumentNullException.ThrowIfNull(array);

            int left = 0,
                right = array.Length - 1;

            while (left < right)
            {
                (array[left], array[right]) = (array[right], array[left]);
                left++;
                right--;
            }
        }

        // 2D -> 1D array
        public static T[] Flatten<T>(T[,] multiArray)
        {
            ArgumentNullException.ThrowIfNull(multiArray);

            int rows = multiArray.GetLength(0);
            int cols = multiArray.GetLength(1);
            T[] result = new T[rows * cols];
            int index = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[index++] = multiArray[i, j];
                }
            }

            return result;
        }

        // jagged -> 2D rectangular array
        public static T[,] ToRectangular<T>(T[][] jaggedArray)
        {
            ArgumentNullException.ThrowIfNull(jaggedArray);

            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;

            for (int i = 1; i < rows; i++)
            {
                if (jaggedArray[i].Length != cols)
                {
                    throw new ArgumentException("Jagged array is not rectangular.");
                }
            }

            T[,] result = new T[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = jaggedArray[i][j];
                }
            }

            return result;
        }

        public class DynamicArray<T>
        {
            private T[] _items;
            private int _count;
            public int Count => _count;
            public int Capacity => _items.Length;

            public DynamicArray(int capacity = 4)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

                _items = new T[capacity];
                _count = 0;
            }

            // gets/sets at specified index.
            public T this[int index]
            {
                get
                {
                    if (index < 0 || index >= _count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    return _items[index];
                }
                set
                {
                    if (index < 0 || index >= _count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }

                    _items[index] = value;
                }
            }

            private void EnsureCapacity(int min)
            {

                if (_items.Length < min)
                {
                    int newCapacity = _items.Length == 0 ? 4 : _items.Length * 2;

                    if (newCapacity < min)
                    {
                        newCapacity = min;
                    }
                    Array.Resize(ref _items, newCapacity);
                }
            }

            // inserts at index.
            public void Insert(int index, T item)
            {
                if (index < 0 || index > _count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                EnsureCapacity(_count + 1);
                Array.Copy(_items, index, _items, index + 1, _count - index);
                _items[index] = item;
                _count++;
            }

            // removes specified index.
            public void RemoveAt(int index)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _count);

                Array.Copy(_items, index + 1, _items, index, _count - index - 1);
                _items[--_count] = default!;
            }

            // add to the end
            public void Add(T item)
            {
                EnsureCapacity(_count + 1);
                _items[_count++] = item;
            }

            public void Clear()
            {
                Array.Clear(_items, 0, _count);
                _count = 0;
            }

            public int IndexOf(T item)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (Equals(_items[i], item))
                    {
                        return i;
                    }
                }

                return -1;
            }

            public bool Contains(T item)
            {
                return IndexOf(item) >= 0;
            }

            // returns string that represents the current dynamic array.
            public override string ToString()
            {
                if (_count == 0)
                {
                    return "[]";
                }

                StringBuilder sb = new();
                sb.Append('[');
                for (int i = 0; i < _count; i++)
                {
                    sb.Append(_items[i]);
                    if (i < _count - 1)
                        sb.Append(", ");
                }
                sb.Append(']');
                return sb.ToString();
            }
        }
    }
}
