using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations.Fails.Exceptions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

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

    public async Task<Result<Guid, Failure>> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createLocationValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors(Errors.LocationErrors.ValidationError);
        }

        var errors = new List<Error>();
        
        var name = Name.Create(request.Name);

        var address = Address.Create(
            request.Address.PostalCode,
            request.Address.Country,
            request.Address.Region,
            request.Address.City,
            request.Address.Street,
            request.Address.House,
            request.Address.Apartment
            );

        if (name.IsFailure)
            errors.AddRange(name.Error);

        if(address.IsFailure)
            errors.AddRange(address.Error);

        if(errors.Count > 0)
            return new Failure(errors);

        var location = Location.Create(name.Value, address.Value);

        if (location.IsFailure)
            return location.Error;

        var result = await _repository.AddAsync(location.Value, cancellationToken);

        if(result.IsFailure)
            return result.Error;

        await _repository.SaveAsync(cancellationToken);

        _logger.LogInformation("Location created with name \"{Name}\".", name);

        return location.Value.Id;
    }

    public async Task<Result<LocationResponse, Failure>> UpdateAsync(Guid id, UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await _updateLocationValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors(Errors.LocationErrors.ValidationError);
        }

        var location = await _repository.GetByIdAsync(id, cancellationToken);

        if (location.IsFailure)
        {
            return Errors.LocationErrors.NotFoud().ToFailure();
        }

        if (request.Name != null)
        {
            var name = Name.Create(request.Name);

            if (name.IsFailure)
                return name.Error;

            if ((await _repository.GetByNameAsync(name.Value, cancellationToken)).IsSuccess)
            {
                return Errors.LocationErrors.ConflictName(name.ToString()).ToFailure();
            }

            location.Value.SetName(name.Value);
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

            if(address.IsFailure)
                return address.Error;

            location.Value.SetAddress(address.Value);
        }

        await _repository.SaveAsync(cancellationToken);

        var addressDto = new AddressDto(
            location.Value.Address.PostalCode,
            location.Value.Address.Country,
            location.Value.Address.Region,
            location.Value.Address.City,
            location.Value.Address.Street,
            location.Value.Address.House,
            location.Value.Address.Apartment);

        return new LocationResponse(
            location.Value.Id,
            location.Value.Name.ToString(),
            addressDto);
    }
}
