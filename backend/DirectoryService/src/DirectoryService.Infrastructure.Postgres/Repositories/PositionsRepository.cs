using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Positions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Linq.Expressions;

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

    public async Task<Result<Position, Failure>> GetByAsync(Expression<Func<Position, bool>> predicate, CancellationToken cancellationToken)
    {
        var position = await _context.Positions.FirstOrDefaultAsync(predicate, cancellationToken);

        if (position == null)
            return Errors.PositionsErrors.NotFoud().ToFailure();

        return position;
    }

    public IAsyncEnumerable<Position> GetByAsyncEnum(Expression<Func<Position, bool>> predicate) =>
        _context.Positions.Where(predicate).AsAsyncEnumerable();
}
