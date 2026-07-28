using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Locations.Update;

internal class UpdateLocationHandler : ICommandHandler<LocationResponse, UpdateLocationCommand>
{
    private readonly ILogger<UpdateLocationHandler> _logger;
    private readonly ILocationsRepository _repository;
    private readonly IValidator<UpdateLocationRequest> _validator;

    public UpdateLocationHandler(ILogger<UpdateLocationHandler> logger, 
                                 ILocationsRepository repository, 
                                 IValidator<UpdateLocationRequest> validator)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<LocationResponse, Failure>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var location = await _repository.GetByIdAsync(command.Id, cancellationToken);

        if (location.IsFailure)
        {
            return Errors.LocationErrors.NotFoud().ToFailure();
        }

        if (command.Request.Name != null)
        {
            var name = Name.Create(command.Request.Name);

            if ((await _repository.GetByNameAsync(name.Value, cancellationToken)).IsSuccess)
            {
                return Errors.LocationErrors.ConflictName(name.ToString()).ToFailure();
            }

            location.Value.SetName(name.Value);
        }

        if (command.Request.Address != null)
        {

            var address = Address.Create(
                command.Request.Address.PostalCode,
                command.Request.Address.Country,
                command.Request.Address.Region,
                command.Request.Address.City,
                command.Request.Address.Street,
                command.Request.Address.House,
                command.Request.Address.Apartment
                );

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

        _logger.LogInformation("The location with ID {Id} was updated.", command.Id);

        return new LocationResponse(
            location.Value.Id,
            location.Value.Name.ToString(),
            addressDto);
    }
}
