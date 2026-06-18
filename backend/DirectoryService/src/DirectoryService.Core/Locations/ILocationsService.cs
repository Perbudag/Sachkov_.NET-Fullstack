using DirectoryService.Contracts.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken = default);
    Task<LocationResponse> UpdateAsync(Guid id, UpdateLocationRequest request, CancellationToken cancellationToken);
}