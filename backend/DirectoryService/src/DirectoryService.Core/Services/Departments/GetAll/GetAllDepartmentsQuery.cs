using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAll;

public record GetAllDepartmentsQuery(
    string? Search = null,
    string SortBy = nameof(DepartmentListItemDto.Id),
    string SortOrder = "ascending",
    int Page = 1,
    int PageSize = 50) : IQuery<GetAllDepartmentsQuery, PageResult<DepartmentListItemDto[]>>
{
    public static implicit operator GetAllDepartmentsQuery(GetAllDepartmentsRequst request) => new(
        Search: request.Search,
        SortBy: request.SortBy,
        SortOrder: request.SortOrder,
        Page: request.Page,
        PageSize: request.PageSize
    );
}