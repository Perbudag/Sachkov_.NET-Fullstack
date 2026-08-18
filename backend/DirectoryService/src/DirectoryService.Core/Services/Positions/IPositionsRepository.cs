using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using Shared;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Positions;

public interface IPositionsRepository
{
    Task<UnitResult<Failure>> AddAsync(Position position, CancellationToken cancellationToken);
    Task<Result<Position, Failure>> GetByAsync(Expression<Func<Position, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken);
    Task<Result<Position, Failure>> GetByAsync(Expression<Func<Position, bool>> predicate, CancellationToken cancellationToken);
    IAsyncEnumerable<Position> GetByAsyncEnum(Expression<Func<Position, bool>> predicate, bool ignoreQueryFilters = false);
}
