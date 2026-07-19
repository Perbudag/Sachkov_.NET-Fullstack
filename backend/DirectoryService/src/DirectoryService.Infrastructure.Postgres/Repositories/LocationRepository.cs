using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Xml.Linq;

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

    public async Task<UnitResult<Failure>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        if (await _context.Locations.AnyAsync(l => l.Name == location.Name, cancellationToken))
        {
            _logger.LogError("Failed to create location with id: {Id}", location.Id);

            return Errors.LocationErrors.ConflictName(location.Name.ToString()).ToFailure();
        }

        await _context.Locations.AddAsync(location, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<Result<Location, Failure>> GetByIdAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location == null)
            return Errors.LocationErrors.NotFoud().ToFailure();

        return location;
    }

    public async Task<Result<IEnumerable<Location>, Failure>> GetByIdsAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken)
    {
        return await _context.Locations.Where(l => locationIds.Contains(l.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Result<Location, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.Name == name, cancellationToken);

        if (location == null)
            return Errors.LocationErrors.NotFoudName().ToFailure();

        return location;
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
