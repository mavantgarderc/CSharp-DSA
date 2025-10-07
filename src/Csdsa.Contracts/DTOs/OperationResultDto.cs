namespace Csdsa.Contracts.Atuh.Dtos;

public record OperationResult<T>(
    bool Success,
    string Message,
    T? Data,
    object? Errors,
    DateTime Timestamp,
    T SuccessResult,
    T ErrorResult
);
