using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Positions.GetAll;

public record GetAllPositionsQuery : IQuery<GetAllPositionsQuery, PositionDto[]>;