namespace Csdsa.Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? CorrelationId { get; set; }
    }

    // public class ApiResponse : ApiResponse<object> { }
}
