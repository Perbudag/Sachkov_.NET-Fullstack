using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Core.Services.Locations;

public interface ILocationsRepository
{
    Task<UnitResult<Failure>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Location>, Failure>> GetByIdsAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
    Task<Result<Location, Failure>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
    Task<Result<Location, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken);
}