using Csdsa.Domain.Common;

namespace Csdsa.Domain.Services.Searching;

public class LinearSearchService
{
    public AlgorithmResponse<T> Search<T>(List<T> items, T target)
    {
        // Linear search logic
        return new AlgorithmResponse<T>(
            result: default,
            complexity: new ComplexityAnalysis("O(n)", "O(1)", "O(n)", "O(n)", "O(n)", "Linear search implementation", 0, items.Count),
            algorithmName: "LinearSearch");
    }
}
