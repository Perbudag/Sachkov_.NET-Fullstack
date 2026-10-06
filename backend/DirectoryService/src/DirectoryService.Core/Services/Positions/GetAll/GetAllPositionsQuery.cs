using DirectoryService.Contracts.Positions;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Positions.GetAll;

public record GetAllPositionsQuery : IQuery<GetAllPositionsQuery, PositionDto[]>;