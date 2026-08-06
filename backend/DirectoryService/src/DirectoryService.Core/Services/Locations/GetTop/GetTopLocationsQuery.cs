using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Locations.GetTop;

public record GetTopLocationsQuery : IQuery<GetTopLocationsQuery, TopLocationResponse[]>;
