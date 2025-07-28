using Csdsa.Domain.Interfaces;

namespace Csdsa.Infrastructure.Services;
public class ArrayTransformationService : IArrayTransformationService
{
    public int[] MergeSortedArrays(int[] arrayA, int[] arrayB)
    {
        if (arrayA == null || arrayA.Length == 0 || arrayB == null || arrayB.Length == 0)
            throw new ArgumentException("Input arrays cannot be null or empty.");

        int i = 0, j = 0, k = 0;
        int[] result = new int[arrayA.Length + arrayB.Length];

        while (i < arrayA.Length && j < arrayB.Length)
        {
            if (arrayA[i] <= arrayB[j])
                result[k++] = arrayA[i++];
            else
                result[k++] = arrayB[j++];
        }

        while (i < arrayA.Length)
            result[k++] = arrayA[i++];

        while (j < arrayB.Length)
            result[k++] = arrayB[j++];

        return result;
    }

    public T[] Flatten<T>(T[,] multiArray)
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

    public T[,] ToRectangular<T>(T[][] jaggedArray)
    {
        ArgumentNullException.ThrowIfNull(jaggedArray);

        int rows = jaggedArray.Length;
        int cols = jaggedArray[0].Length;

        for (int i = 1; i < rows; i++)
        {
            if (jaggedArray[i].Length != cols)
                throw new ArgumentException("Jagged array is not rectangular.");
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
}
