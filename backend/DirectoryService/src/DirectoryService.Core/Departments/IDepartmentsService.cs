using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Guid> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken);
}