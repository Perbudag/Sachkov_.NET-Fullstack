using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;
using System.Transactions;

namespace DirectoryService.Core.Services.Locations.Create;

internal class CreateLocationHandler : ICommandHandler<Guid, CreateLocationCommand>
{
    private readonly ILogger<CreateLocationHandler> _logger;
    private readonly ILocationsRepository _repository;
    private readonly IValidator<CreateLocationRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public CreateLocationHandler(ILogger<CreateLocationHandler> logger,
                                 ILocationsRepository repository,
                                 IValidator<CreateLocationRequest> validator,
                                 ITransactionManager transactionManager)
    {
        _logger = logger;
        _repository = repository;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Failure>> HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var name = Name.Create(command.Request.Name);

        var address = Address.Create(
            command.Request.Address.PostalCode,
            command.Request.Address.Country,
            command.Request.Address.Region,
            command.Request.Address.City,
            command.Request.Address.Street,
            command.Request.Address.House,
            command.Request.Address.Apartment
            );

        var location = Location.Create(name.Value, address.Value);
        

        var result = await _repository.AddAsync(location.Value, cancellationToken);

        if (result.IsFailure)
            return result.Error;

        await _transactionManager.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Location created with name \"{Name}\".", name.Value);

        return location.Value.Id;
    }
}
