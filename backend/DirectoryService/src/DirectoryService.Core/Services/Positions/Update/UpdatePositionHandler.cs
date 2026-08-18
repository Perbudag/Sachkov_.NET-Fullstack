using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Positions.Update;

internal class UpdatePositionHandler : ICommandHandler<PositionDto, UpdatePositionCommand>
{
    private readonly ILogger<UpdatePositionHandler> _logger;
    private readonly IPositionsRepository _repository;
    private readonly IValidator<UpdatePositionRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdatePositionHandler(ILogger<UpdatePositionHandler> logger,
                                 IPositionsRepository repository,
                                 ITransactionManager transactionManager,
                                 IValidator<UpdatePositionRequest> validator)
    {
        _logger = logger;
        _repository = repository;
        _transactionManager = transactionManager;
        _validator = validator;
    }

    public async Task<Result<PositionDto, Failure>> HandleAsync(UpdatePositionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var positionResult = await _repository.GetByAsync(p => p.Id == command.Id && !p.IsDeleted, cancellationToken);

        if (positionResult.IsFailure)
        {
            return positionResult.Error;
        }

        if (command.Request.Name != null)
        {
            var name = Name.Create(command.Request.Name).Value;

            var checkNameResult = await _repository.GetByAsync(p => p.Name == name, cancellationToken);

            if (checkNameResult.IsSuccess && checkNameResult.Value.Id != command.Id)
            {
                return Errors.PositionsErrors.ConflictName(name.ToString()).ToFailure();
            }

            positionResult.Value.SetName(name);
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("The position with ID {Id} was updated.", command.Id);

        return new PositionDto(positionResult.Value.Id, positionResult.Value.Name.ToString());
    }
}
