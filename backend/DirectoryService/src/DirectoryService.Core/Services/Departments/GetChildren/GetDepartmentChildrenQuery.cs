using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetChildren;

public record GetDepartmentChildrenQuery(
    Guid ParentId,
    string SortBy = nameof(DepartmentTreeNodeDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50) : IQuery<GetDepartmentChildrenQuery, PageResult<DepartmentTreeNodeDto[]>>
{
    public static implicit operator GetDepartmentChildrenQuery((Guid parentId, GetDepartmentChildrenRequest request) args)
        => new(args.parentId,
               args.request.SortBy,
               args.request.SortDir,
               args.request.Page,
               args.request.PageSize);
}