namespace Csdsa.Domain.Common;

public class OperationResult<T>
{
    public bool IsSuccess { get; private set; }
    public string? Message { get; private set; }
    public T? Value { get; private set; }
    public object? Errors { get; private set; }
    public DateTime Timestamp { get; private set; }

    protected OperationResult(bool isSuccess, T? value, string? message, object? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Message = message;
        Errors = errors;
        Timestamp = DateTime.UtcNow;
    }

    public static OperationResult<T> SuccessResult(T? value, string message = "Operation successful")
    {
        return new OperationResult<T>(true, value, message, null);
    }

    public static OperationResult<T> ErrorResult(string message, object? errors = null)
    {
        return new OperationResult<T>(false, default, message, errors);
    }
}
