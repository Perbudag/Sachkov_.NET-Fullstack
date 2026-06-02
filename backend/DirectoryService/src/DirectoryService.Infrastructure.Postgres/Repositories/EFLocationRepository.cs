using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class EFLocationRepository : ILocationsRepository
{
    private readonly AppDbContext _appDbContext;
    private readonly ILogger<EFLocationRepository> _logger;

    public EFLocationRepository(AppDbContext appDbContext, ILogger<EFLocationRepository> logger)
    {
        _appDbContext = appDbContext;
        _logger = logger;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _appDbContext.Locations.AddAsync(location, cancellationToken);

            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create location with id: {Id}", location.Id);
            throw;
        }
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _appDbContext.Locations.AnyAsync(location => location.Name == name, cancellationToken);
    }
}
