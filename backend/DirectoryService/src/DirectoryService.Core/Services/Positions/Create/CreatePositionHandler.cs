using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Positions.Create;

internal class CreatePositionHandler : ICommandHandler<Guid, CreatePositionCommand>
{
    private readonly IValidator<CreatePositionRequest> _validator;
    private readonly IPositionsRepository _repository;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<CreatePositionHandler> _logger;

    public CreatePositionHandler(IPositionsRepository repository,
                                 ITransactionManager transactionManager,
                                 IValidator<CreatePositionRequest> validator,
                                 ILogger<CreatePositionHandler> logger)
    {
        _repository = repository;
        _transactionManager = transactionManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> HandleAsync(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var name = Name.Create(command.Request.Name).Value;

        var position = Position.Create(name).Value;

        var result = await _repository.AddAsync(position, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("Position created with name \"{Name}\".", name.Value);

        return position.Id;
    }
}
