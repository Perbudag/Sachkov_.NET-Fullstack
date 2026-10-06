using DirectoryService.Contracts.Departments;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Departments.SearchDepartments;

public record SearchTreeDepartmentsQuery(string Q) : IQuery<SearchTreeDepartmentsQuery, SearchTreeDepartmentsResultDto[]>
{
    public static implicit operator SearchTreeDepartmentsQuery(SearchTreeDepartmentsRequest request) => new(request.q);
}
