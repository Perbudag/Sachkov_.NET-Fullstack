using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.Presenters;

namespace DirectoryService.Core.Services.Departments;

public interface IDepartmentsService
{
    Task<Guid> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task<DepartmentResponse> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken);
    Task AddLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
}