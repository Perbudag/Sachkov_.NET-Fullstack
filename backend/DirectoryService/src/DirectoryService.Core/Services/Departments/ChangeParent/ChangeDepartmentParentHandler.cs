using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Services.Departments.ChangeParent;

internal class ChangeDepartmentParentHandler : ICommandHandler<PutDepartmentParentDto, ChangeDepartmentParentCommand>
{
    private readonly IValidator<ChangeDepartmentParentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;

    public ChangeDepartmentParentHandler(IValidator<ChangeDepartmentParentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<PutDepartmentParentDto, Failure>> HandleAsync(ChangeDepartmentParentCommand command, CancellationToken cancellationToken)
    {
        var validateResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validateResult.IsValid)
        {
            return validateResult.ToErrors();
        }

        var updateResult = await _departmentsRepository.UpdateParentAsync(command.Id, command.Request.ParentId, cancellationToken);


        if (updateResult.IsFailure)
            return updateResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        var department = updateResult.Value;

        return new PutDepartmentParentDto(department.Id,
                                          department.ParentId,
                                          department.Path.ToString(),
                                          department.Depth,
                                          department.UpdatedAt);
    }
}
