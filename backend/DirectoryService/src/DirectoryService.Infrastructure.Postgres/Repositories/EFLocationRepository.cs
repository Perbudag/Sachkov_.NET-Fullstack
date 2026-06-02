using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class EFLocationRepository : ILocationsRepository
{
    private readonly AppDbContext _appDbContext;

    public EFLocationRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _appDbContext.Locations.AddAsync(location, cancellationToken);

        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _appDbContext.Locations.AnyAsync(location => location.Name == name, cancellationToken);
    }
}
