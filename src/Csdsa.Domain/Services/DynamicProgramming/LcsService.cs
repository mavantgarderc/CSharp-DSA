using Csdsa.Domain.Common;
using Csdsa.Domain.Enums;

namespace Csdsa.Domain.Services.DynamicProgramming;

public class LcsService
{
    public AlgorithmResponse<string> LongestCommonSubsequence(string str1, string str2)
    {
        // LCS logic
        return new AlgorithmResponse<string>(
            result: string.Empty,
            complexity: new ComplexityAnalysis("O(mn)", "O(mn)", "O(mn)", "O(mn)", "O(mn)", "LCS implementation", 0, str1.Length),
            algorithmName: AlgorithmType.Lcs.ToString());
    }
}
