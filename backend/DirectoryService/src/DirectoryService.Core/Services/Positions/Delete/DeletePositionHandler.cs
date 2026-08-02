using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using Shared;

namespace DirectoryService.Core.Services.Positions.Delete;

internal class DeletePositionHandler : ICommandHandler<DeletePositionCommand>
{
    private readonly IPositionsRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public DeletePositionHandler(IPositionsRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(DeletePositionCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Errors.SharedErrors.IsRequired("Id", "positions.validation.error").ToFailure();
        }

        var result = await _repository.RemoveAsync(command.Id, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error;
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToFailure();
        }

        return UnitResult.Success<Failure>();
    }
}
