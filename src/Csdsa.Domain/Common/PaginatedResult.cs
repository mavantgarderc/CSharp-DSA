namespace Csdsa.Domain.Common;

public class PaginatedResponse<T> : OperationResult<IEnumerable<T>>
{
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasNextPage { get; private set; }
    public bool HasPreviousPage { get; private set; }

    private PaginatedResponse(
        IEnumerable<T> data,
        int page,
        int pageSize,
        int totalCount,
        string message,
        object? errors)
        : base(true, data, message, errors)
    {
        if (page < 1) throw new ArgumentException("Page number must be positive.", nameof(page));
        if (pageSize < 1) throw new ArgumentException("Page size must be positive.", nameof(pageSize));
        if (totalCount < 0) throw new ArgumentException("Total count cannot be negative.", nameof(totalCount));
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = page < TotalPages;
        HasPreviousPage = page > 1;
    }

    public static PaginatedResponse<T> Create(
        IEnumerable<T> data,
        int page,
        int pageSize,
        int totalCount,
        string message = "Data retrieved successfully")
    {
        return new PaginatedResponse<T>(data, page, pageSize, totalCount, message, null);
    }
}
