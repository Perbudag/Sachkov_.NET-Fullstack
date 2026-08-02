using CSharpFunctionalExtensions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Core.Services.Positions;

public interface IPositionsRepository
{
    Task<UnitResult<Failure>> AddAsync(Position position, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Position>, Failure>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    Task<Result<Position, Failure>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Position, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken);
    Task<UnitResult<Failure>> RemoveAsync(Guid id, CancellationToken cancellationToken);
}
