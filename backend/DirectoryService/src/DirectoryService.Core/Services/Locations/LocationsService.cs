using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations.Fails.Exceptions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Services.Locations;

internal class LocationsService : ILocationsService
{
    private readonly ILogger<LocationsService> _logger;
    private readonly ILocationsRepository _repository;
    private readonly IValidator<CreateLocationRequest> _createLocationValidator;
    private readonly IValidator<UpdateLocationRequest> _updateLocationValidator;

    public LocationsService(ILogger<LocationsService> logger,
                            ILocationsRepository repository,
                            IValidator<CreateLocationRequest> createLocationValidator,
                            IValidator<UpdateLocationRequest> updateLocationValidator)
    {
        _logger = logger;
        _repository = repository;
        _createLocationValidator = createLocationValidator;
        _updateLocationValidator = updateLocationValidator;
    }

    public async Task<Guid> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createLocationValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrors(Errors.LocationErrors.ValidationError);

            throw new LocationsValidationException(errors);
        }

        var name = Name.Create(request.Name);

        if (await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            var error = Errors.LocationErrors.Conflict(name.ToString());

            throw new LocationsConflictException(error);
        }

        var address = Address.Create(
            request.Address.PostalCode,
            request.Address.Country,
            request.Address.Region,
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment
            );

        var location = Location.Create(name, address);

        await _repository.AddAsync(location, cancellationToken);

        await _repository.SaveAsync(cancellationToken);

        _logger.LogInformation("Location created with name \"{Name}\".", name);

        return location.Id;
    }

    public async Task<LocationResponse> UpdateAsync(Guid id, UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrors(Errors.LocationErrors.ValidationError);

            throw new LocationsValidationException(errors);
        }

        var location = await _repository.GetByIdAsync(id, cancellationToken);

        if (location == null)
        {
            var error = Errors.LocationErrors.NotFoud();

            throw new LocationsNotFoundException(error);
        }

        if (request.Name != null)
        {
            var name = Name.Create(request.Name);

            if (await _repository.ExistsByNameAsync(name, cancellationToken))
            {
                var error = Errors.LocationErrors.Conflict(name.ToString());

                throw new LocationsConflictException(error);
            }

            location.SetName(name);
        }

        if (request.Address != null)
        {

            var address = Address.Create(
                request.Address.PostalCode,
                request.Address.Country,
                request.Address.Region,
                request.Address.City,
                request.Address.Street,
                request.Address.House,
                request.Address.Apartment
                );

            location.SetAddress(address);
        }

        await _repository.SaveAsync(cancellationToken);

        var addressDto = new AddressDto(
            location.Address.PostalCode,
            location.Address.Country,
            location.Address.Region,
            location.Address.City,
            location.Address.Street,
            location.Address.House,
            location.Address.Apartment);

        return new LocationResponse(
            location.Id,
            location.Name.ToString(),
            addressDto);
    }
}
