using Csdsa.Domain.Common;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.Sorting;

public class ComparisonSortService
{
    public AlgorithmResponse<List<T>> QuickSort<T>(List<T> items) where T : IComparable<T>
    {
        // QuickSort logic
        return new AlgorithmResponse<List<T>>(
            result: items,
            complexity: new ComplexityAnalysis("O(n log n)", "O(log n)", "O(n log n)", "O(n log n)", "O(n²)", "QuickSort implementation", 0, items.Count),
            algorithmName: AlgorithmType.QuickSort.ToString());
    }

    public AlgorithmResponse<List<T>> MergeSort<T>(List<T> items) where T : IComparable<T>
    {
        // MergeSort logic
        return new AlgorithmResponse<List<T>>(
            result: items,
            complexity: new ComplexityAnalysis("O(n log n)", "O(n)", "O(n log n)", "O(n log n)", "O(n log n)", "MergeSort implementation", 0, items.Count),
            algorithmName: AlgorithmType.MergeSort.ToString());
    }
}
