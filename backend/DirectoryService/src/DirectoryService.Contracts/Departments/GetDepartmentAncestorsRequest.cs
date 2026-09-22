namespace DirectoryService.Contracts.Departments;

public record GetDepartmentAncestorsRequest(
    string SortBy = nameof(DepartmentTreeNodeDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50);
