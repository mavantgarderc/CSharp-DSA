namespace Csdsa.Domain.Interfaces;
/// <summary>
/// Service for searching operations within arrays
/// </summary>
public interface IArraySearchService
{
    /// <summary>
    /// Finds the maximum element in the array
    /// </summary>
    int FindMax(int[] array);

    /// <summary>
    /// Finds the minimum element in the array
    /// </summary>
    int FindMin(int[] array);

    /// <summary>
    /// Performs linear search for target element
    /// </summary>
    /// <returns>Index of element or -1 if not found</returns>
    int LinearSearch(int[] array, int target);

    /// <summary>
    /// Performs binary search for target element (array must be sorted)
    /// </summary>
    /// <returns>Index of element or -1 if not found</returns>
    int BinarySearch(int[] array, int target);
}
