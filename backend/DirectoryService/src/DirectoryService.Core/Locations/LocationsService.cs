using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Locations;

internal class LocationsService : ILocationsService
{
    private readonly ILogger<LocationsService> _logger;
    private readonly ILocationsRepository _repository;
    private readonly IValidator<CreateLocationRequest> _createLocationValidator;

    public LocationsService(ILogger<LocationsService> logger,
                            ILocationsRepository repository,
                            IValidator<CreateLocationRequest> createLocationValidator)
    {
        _logger = logger;
        _repository = repository;
        _createLocationValidator = createLocationValidator;
    }

    public async Task<Guid> CreateAsync(CreateLocationRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createLocationValidator.ValidateAsync(request, cancellationToken);
        if(!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var name = Name.Create(request.Name);

        if(!await _repository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ValidationException($"A location named \"{name}\" already exists");
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

        // Если пользователь с именем request.Name уже существует бд вызовет исключение
        await _repository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Location created with name \"{Name}\".", name);

        return location.Id;
    }
}
