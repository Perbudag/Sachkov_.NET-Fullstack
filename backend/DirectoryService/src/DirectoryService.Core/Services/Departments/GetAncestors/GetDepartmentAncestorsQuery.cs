using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Services.Departments.GetChildren;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAncestors;

public record GetDepartmentAncestorsQuery(
    Guid ParentId,
    string SortBy = nameof(DepartmentTreeNodeDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50) : IQuery<GetDepartmentAncestorsQuery, PageResult<DepartmentTreeNodeDto[]>>
{
    public static implicit operator GetDepartmentAncestorsQuery((Guid parentId, GetDepartmentAncestorsRequest request) args)
        => new(args.parentId,
               args.request.SortBy,
               args.request.SortDir,
               args.request.Page,
               args.request.PageSize);
}