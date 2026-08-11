using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core.Services.Locations.GetAll;

public record GetAllLocationsDapperQuery(
    string? Search,
    int MinDepartmentCount = 0,
    string SortBy = nameof(LocationListItemDto.Id),
    string SortDir = "asc",
    int Page = 1,
    int PageSize = 50
    ) : IQuery<GetAllLocationsDapperQuery, PageResult<LocationListItemDto[]>>
{
    public static implicit operator GetAllLocationsDapperQuery(GetAllLocationsRequest request) =>
        new(request.Search, request.MinDepartmentCount, request.SortBy, request.SortDir, request.Page, request.PageSize);
}
