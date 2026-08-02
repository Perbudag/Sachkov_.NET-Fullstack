using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Departments.GetById;

public record GetByIdDepartmentQuery(Guid Id) : IQuery<GetByIdDepartmentQuery, DepartmentResponse>
{
    public static implicit operator GetByIdDepartmentQuery(Guid id) => new(id);
}
