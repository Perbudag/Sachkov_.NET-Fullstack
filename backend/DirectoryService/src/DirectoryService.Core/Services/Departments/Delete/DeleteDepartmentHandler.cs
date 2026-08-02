using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using Shared;

namespace DirectoryService.Core.Services.Departments.Delete;

internal class DeleteDepartmentHandler : ICommandHandler<DeleteDepartmentCommand>
{
    private readonly IDepartmentsRepository _repository;
    private readonly ITransactionManager _transactionManager;

    public DeleteDepartmentHandler(IDepartmentsRepository repository, ITransactionManager transactionManager)
    {
        _repository = repository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(DeleteDepartmentCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            return Errors.SharedErrors.IsRequired("Id", "departments.validation.error").ToFailure();
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
