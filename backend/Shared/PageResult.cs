namespace Shared;

public record PageResult<T>(
    T Value,
    long TotalCount,
    int PageNumber,
    int PageSize
);