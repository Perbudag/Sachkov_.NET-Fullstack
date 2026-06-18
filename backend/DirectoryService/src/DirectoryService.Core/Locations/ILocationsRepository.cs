using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task AddAsync(Location location, CancellationToken cancellationToken);
    Task<IEnumerable<Location>> GetByIdsAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken);
    Task SaveAsync(CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken);
}