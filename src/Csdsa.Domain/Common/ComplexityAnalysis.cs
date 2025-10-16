namespace Csdsa.Domain.Common;

public class ComplexityAnalysis
{
    public string TimeComplexity { get; private set; }
    public string SpaceComplexity { get; private set; }
    public string BestCase { get; private set; }
    public string AverageCase { get; private set; }
    public string WorstCase { get; private set; }
    public string Description { get; private set; }
    public long ExecutionTimeMs { get; private set; }
    public int DataSize { get; private set; }

    public ComplexityAnalysis(
        string timeComplexity,
        string spaceComplexity,
        string bestCase,
        string averageCase,
        string worstCase,
        string description,
        long executionTimeMs,
        int dataSize)
    {
        TimeComplexity = timeComplexity ?? throw new ArgumentNullException(nameof(timeComplexity));
        SpaceComplexity = spaceComplexity ?? throw new ArgumentNullException(nameof(spaceComplexity));
        BestCase = bestCase ?? throw new ArgumentNullException(nameof(bestCase));
        AverageCase = averageCase ?? throw new ArgumentNullException(nameof(averageCase));
        WorstCase = worstCase ?? throw new ArgumentNullException(nameof(worstCase));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        if (executionTimeMs < 0) throw new ArgumentException("Execution time cannot be negative.", nameof(executionTimeMs));
        if (dataSize < 0) throw new ArgumentException("Data size cannot be negative.", nameof(dataSize));
        ExecutionTimeMs = executionTimeMs;
        DataSize = dataSize;
    }
}
