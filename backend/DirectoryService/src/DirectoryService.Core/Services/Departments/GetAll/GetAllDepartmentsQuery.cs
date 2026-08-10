using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAll;

public record GetAllDepartmentsQuery(
    string? Search = null,
    string SortBy = nameof(DepartmentListItemDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50) : IQuery<GetAllDepartmentsQuery, PageResult<DepartmentListItemDto[]>>
{
    public static implicit operator GetAllDepartmentsQuery(GetAllDepartmentsRequest request) => new(
        Search: request.Search,
        SortBy: request.SortBy,
        SortDir: request.SortDir,
        Page: request.Page,
        PageSize: request.PageSize
    );
}