using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using Shared.Core.CQRS;
using Shared.Kernel;

namespace DirectoryService.Core.Services.Locations.GetById;

internal class GetByIdLocationHandler : IQueryHandler<LocationDto, GetByIdLocationQuery>
{
    private readonly IReadDbContext _context;

    public GetByIdLocationHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<LocationDto, Failure>> HandleAsync(GetByIdLocationQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
            return SharedErrors.IsRequired("Id", "locations.validation.error").ToFailure();


        var response = await _context.LocationsRead
            .Where(d => d.Id == query.Id)
            .Select(d => new LocationDto(
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
            .FirstOrDefaultAsync(cancellationToken);


        if (response == null)
            return LocationErrors.NotFoud().ToFailure();


        return response;
    }
}
