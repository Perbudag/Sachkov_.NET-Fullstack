using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using Shared;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Locations;

public interface ILocationsRepository
{
    Task<UnitResult<Failure>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<Result<Location, Failure>> GetByAsync(Expression<Func<Location, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken);
    Task<Result<Location, Failure>> GetByAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken);
    IAsyncEnumerable<Location> GetByAsyncEnum(Expression<Func<Location, bool>> predicate, bool ignoreQueryFilters = false);
}