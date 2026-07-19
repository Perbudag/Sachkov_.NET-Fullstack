using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.Presenters;
using Shared;

namespace DirectoryService.Core.Services.Departments;

public interface IDepartmentsService
{
    Task<UnitResult<Failure>> AddLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<Result<Guid, Failure>> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken);
    Task<Result<DepartmentResponse, Failure>> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken);
}