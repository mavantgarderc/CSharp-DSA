namespace Csdsa.Contracts.Dtos;

public record AlgorithmResponseDto<T>(
    T Result,
    ComplexityDto Complexity,
    string AlgorithmName,
    Dictionary<string, string> Metadata
);
