using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Positions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Xml.Linq;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class PositionsRepository : IPositionsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<PositionsRepository> _logger;

    public PositionsRepository(AppDbContext context, ILogger<PositionsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UnitResult<Failure>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        if (await _context.Positions.AnyAsync(l => l.Name == position.Name, cancellationToken))
        {
            _logger.LogError("Failed to create position with id: {Id}", position.Id);

            return Errors.PositionsErrors.ConflictName(position.Name.ToString()).ToFailure();
        }

        await _context.Positions.AddAsync(position, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<Result<Position, Failure>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (position == null)
            return Errors.LocationErrors.NotFoud().ToFailure();

        return position;
    }

    public async Task<Result<IEnumerable<Position>, Failure>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        return await _context.Positions.Where(l => ids.Contains(l.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Result<Position, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(d => d.Name == name, cancellationToken);

        if (position == null)
            return Errors.DepartmentErrors.NotFoudName().ToFailure();

        return position;
    }

    public async Task<UnitResult<Failure>> RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (position == null)
            return Errors.PositionsErrors.NotFoud().ToFailure();

        _context.Positions.Remove(position);

        return UnitResult.Success<Failure>();
    }
}
