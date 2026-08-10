namespace DirectoryService.Contracts.Departments;

public record GetAllDepartmentsRequst(
    string? Search = null,
    string SortBy = nameof(DepartmentListItemDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50);