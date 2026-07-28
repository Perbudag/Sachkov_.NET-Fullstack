using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Core.Services.Departments;

public interface IDepartmentsRepository
{
    Task<UnitResult<Failure>> AddAsync(Department department, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Department>, Failure>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> RemoveLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
}