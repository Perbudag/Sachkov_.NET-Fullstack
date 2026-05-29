using DirectoryService.Domain.Entities;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task AddAsync(Location location, CancellationToken cancellationToken);
}