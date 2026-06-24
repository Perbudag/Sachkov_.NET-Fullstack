using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Services.Departments;

public interface IDepartmentsRepository
{
    Task AddAsync(Department department, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken);
    Task<bool> ExistsChildWithSlugAsync(Department parent, Slug slug, CancellationToken cancellationToken);
    Task<Department?> GetByIdAsync(Guid? departmentId, CancellationToken cancellationToken);
    Task AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
    Task RemoveLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
    Task<bool> LocationExistsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
}