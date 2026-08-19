using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Locations.GetTop;

internal class GetTopLocationsHandler : IQueryHandler<LocationListItemDto[], GetTopLocationsQuery>
{
    private readonly IReadDbContext _context;

    public GetTopLocationsHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LocationListItemDto[], Failure>> HandleAsync(GetTopLocationsQuery query, CancellationToken cancellationToken)
    {
        var response = await (from departmentLocation in _context.DepartmentLocationsRead
                              join department in _context.DepartmentsRead
                                  on departmentLocation.DepartmentId equals department.Id
                              where !department.IsDeleted
                              join location in _context.LocationsRead
                                  on departmentLocation.LocationId equals location.Id
                              group departmentLocation by location into locationGroup
                              orderby locationGroup.Count() descending
                              select new LocationListItemDto(
                                        id: locationGroup.Key.Id,
                                        name: locationGroup.Key.Name.ToString(),
                                        address: new AddressDto(
                                            PostalCode: locationGroup.Key.Address.PostalCode,
                                            Country: locationGroup.Key.Address.Country,
                                            Region: locationGroup.Key.Address.Region,
                                            City: locationGroup.Key.Address.City,
                                            Street: locationGroup.Key.Address.Street,
                                            House: locationGroup.Key.Address.House,
                                            Apartment: locationGroup.Key.Address.Apartment),
                                        createdAt: locationGroup.Key.CreatedAt,
                                        departmentCount: locationGroup.Count())
                        )
                        .Take(5)
                        .ToArrayAsync(cancellationToken);

        return response;
    }
}
