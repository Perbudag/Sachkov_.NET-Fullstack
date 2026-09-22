using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetDepartmentsTree;

public record GetDepartmentsTreeQuery(
    string SortBy = nameof(DepartmentTreeNodeDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50) : IQuery<GetDepartmentsTreeQuery, PageResult<DepartmentTreeNodeDto[]>>
{
    public static implicit operator GetDepartmentsTreeQuery(GetDepartmentTreeNodeRequest request)
        => new(request.SortBy, request.SortDir, request.Page, request.PageSize);

}
