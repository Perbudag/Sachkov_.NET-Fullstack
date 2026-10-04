using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.Errors;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Services.Departments.ChangeParent;

internal class MoveDepartmentHandler : ICommandHandler<MoveDepartmentDto, MoveDepartmentCommand>
{
    private readonly IValidator<MoveDepartmentCommand> _validator;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ITransactionManager _transactionManager;

    public MoveDepartmentHandler(IValidator<MoveDepartmentCommand> validator,
        IDepartmentsRepository departmentsRepository,
        ITransactionManager transactionManager)
    {
        _validator = validator;
        _departmentsRepository = departmentsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<Result<MoveDepartmentDto, Failure>> HandleAsync(MoveDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validateResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validateResult.IsValid)
        {
            return validateResult.ToErrors();
        }

        var getDepartmentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.Id, cancellationToken);

        if (getDepartmentResult.IsFailure)
        {
            return getDepartmentResult.Error;
        }

        var department = getDepartmentResult.Value;
        Department? parent = null;

        if (command.Request.ParentId != null)
        {
            var getParentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.Request.ParentId, true, cancellationToken);

            if (getParentResult.IsFailure)
            {
                return DepartmentErrors.NotFoudParent().ToFailure();
            }
            else if(getParentResult.Value.IsDeleted)
            {
                return DepartmentErrors.MoveParentIsDeleted().ToFailure();
            }
            
            parent = getParentResult.Value;
        }

        var updateResult = await _departmentsRepository.MoveAsync(department, parent, cancellationToken);


        if (updateResult.IsFailure)
            return updateResult.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        return new MoveDepartmentDto(department.Id,
                                          department.ParentId,
                                          department.Path.ToString(),
                                          department.Depth,
                                          department.UpdatedAt);
    }
}
