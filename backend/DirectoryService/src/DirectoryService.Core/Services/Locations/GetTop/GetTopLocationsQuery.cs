using DirectoryService.Contracts.Locations;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Locations.GetTop;

public record GetTopLocationsQuery : IQuery<GetTopLocationsQuery, LocationListItemDto[]>;
