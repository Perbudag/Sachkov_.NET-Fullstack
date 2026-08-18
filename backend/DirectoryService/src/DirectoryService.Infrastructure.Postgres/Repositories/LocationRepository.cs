using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Linq.Expressions;

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
        if (await _context.Locations.IgnoreQueryFilters().AnyAsync(l => l.Name == location.Name, cancellationToken))
        {
            _logger.LogError("Failed to create location with id: {Id}", location.Id);

            return Errors.LocationErrors.ConflictName(location.Name.ToString()).ToFailure();
        }

        await _context.Locations.AddAsync(location, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<Result<Location, Failure>> GetByAsync(Expression<Func<Location, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken)
    {
        var query = _context.Locations;

        if (ignoreQueryFilters)
        {
            query.IgnoreQueryFilters();
        }

        var location = await query.FirstOrDefaultAsync(predicate, cancellationToken);

        if (location == null)
            return Errors.LocationErrors.NotFoud().ToFailure();

        return location;
    }

    public Task<Result<Location, Failure>> GetByAsync(Expression<Func<Location, bool>> predicate, CancellationToken cancellationToken) =>
        GetByAsync(predicate, false, cancellationToken);

    public IAsyncEnumerable<Location> GetByAsyncEnum(Expression<Func<Location, bool>> predicate, bool ignoreQueryFilters = false)
    {
        var query = _context.Locations;

        if(ignoreQueryFilters)
        {
            query.IgnoreQueryFilters();
        }    

        return query.Where(predicate).AsAsyncEnumerable();
    }
}