namespace Csdsa.Domain.Interfaces;

/// <summary>
/// Service for array manipulation operations like rotation and reversal
/// </summary>
public interface IArrayManipulationService
{
    /// <summary>
    /// Rotates array elements to the right by k positions
    /// </summary>
    void RotateRight(int[] array, int k);

    /// <summary>
    /// Rotates array elements to the left by k positions
    /// </summary>
    void RotateLeft(int[] array, int k);

    /// <summary>
    /// Reverses the array in-place
    /// </summary>
    void Reverse<T>(T[] array);
}
