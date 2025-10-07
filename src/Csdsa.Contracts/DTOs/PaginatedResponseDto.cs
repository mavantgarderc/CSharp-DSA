namespace Csdsa.Contracts.Dtos;

public record PaginatedResponseDto<T>(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage,
    T Create
);
