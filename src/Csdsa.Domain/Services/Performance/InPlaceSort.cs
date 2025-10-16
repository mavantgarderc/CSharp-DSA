using Csdsa.Domain.Common;

namespace Csdsa.Domain.Services.Performance;

public class InPlaceSort
{
    public AlgorithmResponse<List<T>> Sort<T>(List<T> items) where T : IComparable<T>
    {
        // In-place sort logic
        return new AlgorithmResponse<List<T>>(
            result: items,
            complexity: new ComplexityAnalysis("O(n log n)", "O(1)", "O(n log n)", "O(n log n)", "O(n²)", "In-place sort implementation", 0, items.Count),
            algorithmName: "InPlaceSort");
    }
}
