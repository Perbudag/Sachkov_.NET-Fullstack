using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Locations.GetTop;

internal class GetTopLocationsHandler : IQueryHandler<TopLocationResponse[], GetTopLocationsQuery>
{
    private readonly IReadDbContext _context;

    public GetTopLocationsHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TopLocationResponse[], Failure>> HandleAsync(GetTopLocationsQuery query, CancellationToken cancellationToken)
    {
        var response = await (from departmentLocation in _context.DepartmentLocationsRead
                              join location in _context.LocationsRead
                                  on departmentLocation.LocationId equals location.Id
                              group departmentLocation by location into locationGroup
                              orderby locationGroup.Count() descending
                              select new TopLocationResponse(
                                        Id: locationGroup.Key.Id,
                                        Name: locationGroup.Key.Name.ToString(),
                                        Address: new AddressDto(
                                            PostalCode: locationGroup.Key.Address.PostalCode,
                                            Country: locationGroup.Key.Address.Country,
                                            Region: locationGroup.Key.Address.Region,
                                            City: locationGroup.Key.Address.City,
                                            Street: locationGroup.Key.Address.Street,
                                            House: locationGroup.Key.Address.House,
                                            Apartment: locationGroup.Key.Address.Apartment),
                                        DepartmentCount: locationGroup.Count())
                        )
                        .Take(5)
                        .ToArrayAsync(cancellationToken);

        return response;
    }
}
