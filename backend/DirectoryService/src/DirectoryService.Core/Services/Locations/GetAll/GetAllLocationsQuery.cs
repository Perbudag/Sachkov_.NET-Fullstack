using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using System.Windows.Input;

namespace DirectoryService.Core.Services.Locations.GetAll;

public record GetAllLocationsQuery : IQuery<GetAllLocationsQuery, LocationResponse[]>;
