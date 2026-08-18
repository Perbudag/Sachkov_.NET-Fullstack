using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Locations.Update;

internal class UpdateLocationHandler : ICommandHandler<LocationDto, UpdateLocationCommand>
{
    private readonly ILogger<UpdateLocationHandler> _logger;
    private readonly ILocationsRepository _repository;
    private readonly IValidator<UpdateLocationRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateLocationHandler(ILogger<UpdateLocationHandler> logger,
                                 ILocationsRepository repository,
                                 IValidator<UpdateLocationRequest> validator,
                                 ITransactionManager transactionManager)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<LocationDto, Failure>> HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var locationResult = await _repository.GetByAsync(l => l.Id == command.Id && !l.IsDeleted, cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error;
        }

        if (command.Request.Name != null)
        {
            var name = Name.Create(command.Request.Name);

            if ((await _repository.GetByAsync(l => l.Name == name.Value, cancellationToken)).IsSuccess)
            {
                return Errors.LocationErrors.ConflictName(name.ToString()).ToFailure();
            }

            locationResult.Value.SetName(name.Value);
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
                ).Value;

            locationResult.Value.SetAddress(address);
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        var addressDto = new AddressDto(
            locationResult.Value.Address.PostalCode,
            locationResult.Value.Address.Country,
            locationResult.Value.Address.Region,
            locationResult.Value.Address.City,
            locationResult.Value.Address.Street,
            locationResult.Value.Address.House,
            locationResult.Value.Address.Apartment);

        _logger.LogInformation("The location with ID {Id} was updated.", command.Id);

        return new LocationDto(
            locationResult.Value.Id,
            locationResult.Value.Name.ToString(),
            addressDto);
    }
}
