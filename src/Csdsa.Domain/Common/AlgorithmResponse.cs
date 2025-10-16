namespace Csdsa.Domain.Common;

public class AlgorithmResponse<T>
{
    public T Result { get; private set; }
    public ComplexityAnalysis Complexity { get; private set; }
    public string AlgorithmName { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }

    public AlgorithmResponse(T result, ComplexityAnalysis complexity, string algorithmName, Dictionary<string, string>? metadata = null)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Complexity = complexity ?? throw new ArgumentNullException(nameof(complexity));
        AlgorithmName = algorithmName ?? throw new ArgumentNullException(nameof(algorithmName));
        Metadata = metadata ?? new Dictionary<string, string>();
    }
}
