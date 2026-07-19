using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using Shared;

namespace DirectoryService.Core.Services.Locations;

public interface ILocationsService
{
    Task<Result<Guid, Failure>> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken = default);
    Task<Result<LocationResponse, Failure>> UpdateAsync(Guid id, UpdateLocationRequest request, CancellationToken cancellationToken);
}