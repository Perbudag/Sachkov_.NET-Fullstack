using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.SearchDepartments;

public record SearchTreeDepartmentsQuery(string Q) : IQuery<SearchTreeDepartmentsQuery, SearchTreeDepartmentsResultDto[]>
{
    public static implicit operator SearchTreeDepartmentsQuery(SearchTreeDepartmentsRequest request) => new(request.q);
}
