using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class LocationRepository : ILocationsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<LocationRepository> _logger;

    public LocationRepository(AppDbContext appDbContext, ILogger<LocationRepository> logger)
    {
        _context = appDbContext;
        _logger = logger;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Locations.AddAsync(location, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create location with id: {Id}", location.Id);
            throw;
        }
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _context.Locations.AnyAsync(location => location.Name == name, cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetByIdsAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken)
    {
        return await _context.Locations.Where(l => locationIds.Contains(l.Id)).ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
