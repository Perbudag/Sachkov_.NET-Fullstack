using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using Shared;
using Shared.Kernel;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Departments;

public interface IDepartmentsRepository
{
    Task<UnitResult<Failure>> AddAsync(Department department, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> MoveAsync(Department department, Department? newParent, CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetByAsync(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetWithLockByAsync(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken);
    Task<Result<Department, Failure>> GetWithLockByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken);
    IAsyncEnumerable<Department> GetByAsyncEnum(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters = false);
    Task<UnitResult<Failure>> AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> RemoveLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> AddPositionsAsync(Department department, IEnumerable<Position> positions, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> RemovePositionsAsync(Department department, IEnumerable<Position> positions, CancellationToken cancellationToken);
    Task<long> CountByAsync(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken);
    Task<long> CountByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken);
}