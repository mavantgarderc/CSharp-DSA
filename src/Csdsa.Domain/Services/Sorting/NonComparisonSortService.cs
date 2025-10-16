using Csdsa.Domain.Common;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.Sorting;

public class NonComparisonSortService
{
    public AlgorithmResponse<List<int>> CountingSort(List<int> items)
    {
        // CountingSort logic
        return new AlgorithmResponse<List<int>>(
            result: items,
            complexity: new ComplexityAnalysis("O(n + k)", "O(k)", "O(n + k)", "O(n + k)", "O(n + k)", "CountingSort implementation", 0, items.Count),
            algorithmName: AlgorithmType.CountingSort.ToString());
    }

    public AlgorithmResponse<List<int>> RadixSort(List<int> items)
    {
        // RadixSort logic
        return new AlgorithmResponse<List<int>>(
            result: items,
            complexity: new ComplexityAnalysis("O(nk)", "O(n + k)", "O(nk)", "O(nk)", "O(nk)", "RadixSort implementation", 0, items.Count),
            algorithmName: AlgorithmType.RadixSort.ToString());
    }
}
