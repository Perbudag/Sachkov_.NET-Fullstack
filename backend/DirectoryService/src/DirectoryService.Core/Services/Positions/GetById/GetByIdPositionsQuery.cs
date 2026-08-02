using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Positions.GetById;

public record GetByIdPositionsQuery(Guid Id) : IQuery<GetByIdPositionsQuery, PositionResponse>
{
    public static implicit operator GetByIdPositionsQuery(Guid id) => new(id);
}
