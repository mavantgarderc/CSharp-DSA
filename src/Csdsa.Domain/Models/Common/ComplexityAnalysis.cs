namespace Csdsa.Domain.Models.Common
{
    public class ComplexityAnalysis
    {
        public string TimeComplexity { get; set; } = string.Empty;
        public string SpaceComplexity { get; set; } = string.Empty;
        public string BestCase { get; set; } = string.Empty;
        public string AverageCase { get; set; } = string.Empty;
        public string WorstCase { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
        public int DataSize { get; set; }
    }

    public class AlgorithmResponse<T>
    {
        public T Result { get; set; } = default(T)!;
        public ComplexityAnalysis Complexity { get; set; } = new();
        public string AlgorithmName { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
