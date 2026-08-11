namespace DirectoryService.Contracts.Locations;

public record GetAllLocationsRequest(
    string? Search,
    int MinDepartmentCount = 0,
    string SortBy = nameof(LocationListItemDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50);