using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Locations.GetAll;

public record GetAllLocationsQuery : IQuery<GetAllLocationsQuery, LocationDto[]>;
