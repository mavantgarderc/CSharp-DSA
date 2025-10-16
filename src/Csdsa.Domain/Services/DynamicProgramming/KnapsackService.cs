using Csdsa.Domain.Common;
using Csdsa.Domain.Entities;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.DynamicProgramming;

public class KnapsackService
{
    public AlgorithmResponse<int> Knapsack(int capacity, List<int> weights, List<int> values)
    {
        // Knapsack logic
        return new AlgorithmResponse<int>(
            result: 0,
            complexity: new ComplexityAnalysis("O(nW)", "O(nW)", "O(nW)", "O(nW)", "O(nW)", "Knapsack implementation", 0, weights.Count),
            algorithmName: AlgorithmType.Knapsack.ToString());
    }
}
