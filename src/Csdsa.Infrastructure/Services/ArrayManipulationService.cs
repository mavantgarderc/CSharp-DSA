using Csdsa.Domain.Interfaces;

namespace Csdsa.Infrastructure.Services;

public class ArrayManipulationService : IArrayManipulationService
{
    public void RotateRight(int[] array, int k)
    {
        if (array == null || array.Length == 0)
            return;

        int n = array.Length;
        k %= n;
        Reverse(array, 0, n - 1);
        Reverse(array, 0, k - 1);
        Reverse(array, k, n - 1);
    }

    public void RotateLeft(int[] array, int k)
    {
        if (array == null || array.Length == 0)
            return;

        int n = array.Length;
        k %= n;
        Reverse(array, 0, k - 1);
        Reverse(array, k, n - 1);
        Reverse(array, 0, n - 1);
    }

    public void Reverse<T>(T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        int left = 0;
        int right = array.Length - 1;

        while (left < right)
        {
            (array[left], array[right]) = (array[right], array[left]);
            left++;
            right--;
        }
    }

    private static void Reverse(int[] arr, int start, int end)
    {
        while (start < end)
        {
            (arr[start], arr[end]) = (arr[end], arr[start]);
            start++;
            end--;
        }
    }
}
