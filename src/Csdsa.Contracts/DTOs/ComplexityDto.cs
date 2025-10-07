namespace Csdsa.Contracts.Dtos;

public record ComplexityDto(
    string TimeComplexity,
    string SpaceComplexity,
    string BestCase,
    string AverageCase,
    string WorstCase,
    string Description,
    long ExecutionTimeMs,
    int DataSize
);
