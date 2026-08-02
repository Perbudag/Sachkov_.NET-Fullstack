using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Locations.GetAll;

internal class GetAllLocationsHandler : IQueryHandler<LocationResponse[], GetAllLocationsQuery>
{
    private readonly IReadDbContext _context;

    public GetAllLocationsHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LocationResponse[], Failure>> HandleAsync(GetAllLocationsQuery query, CancellationToken cancellationToken)
    {
        var responses = await _context.LocationsRead
            .Select(d => new LocationResponse(
                Id: d.Id,
                Name: d.Name.ToString(),
                Address: new AddressDto(
                    PostalCode: d.Address.PostalCode,
                    Country: d.Address.Country,
                    Region: d.Address.Region,
                    City: d.Address.City,
                    Street: d.Address.Street,
                    House: d.Address.House,
                    Apartment: d.Address.Apartment)
            ))
            .ToArrayAsync(cancellationToken);


        return responses;
    }
}
