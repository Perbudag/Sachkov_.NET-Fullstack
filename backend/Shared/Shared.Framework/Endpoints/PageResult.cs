namespace Shared.Framework.Endpoints;

public record PageResult<T>(
    T Value,
    long TotalCount,
    int PageNumber,
    int PageSize
);