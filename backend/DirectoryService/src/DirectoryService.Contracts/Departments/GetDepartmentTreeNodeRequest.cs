namespace DirectoryService.Contracts.Departments;

public record GetDepartmentTreeNodeRequest(
    string SortBy = nameof(DepartmentTreeNodeDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50);
