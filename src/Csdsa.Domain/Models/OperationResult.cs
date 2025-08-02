namespace Csdsa.Domain.Models;

public class OperationResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public DateTime Timestamp { get; set; }

    public static OperationResult<T> SuccessResult(T data, string message = "Operation successful")
    {
        return new OperationResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    public static OperationResult<T> ErrorResult(string message, object? errors = null)
    {
        return new OperationResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}
