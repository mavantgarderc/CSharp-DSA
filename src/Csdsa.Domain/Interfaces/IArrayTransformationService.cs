namespace Csdsa.Domain.Interfaces;
/// <summary>
/// Service for array transformation and merging operations
/// </summary>
public interface IArrayTransformationService
{
    /// <summary>
    /// Merges two sorted arrays into one sorted array
    /// </summary>
    int[] MergeSortedArrays(int[] arrayA, int[] arrayB);

    /// <summary>
    /// Flattens a 2D rectangular array into a 1D array
    /// </summary>
    T[] Flatten<T>(T[,] multiArray);

    /// <summary>
    /// Converts a jagged array to a rectangular 2D array
    /// </summary>
    T[,] ToRectangular<T>(T[][] jaggedArray);
}
