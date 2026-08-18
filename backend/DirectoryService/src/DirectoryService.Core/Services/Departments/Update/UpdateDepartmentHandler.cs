using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.Update;

internal class UpdateDepartmentHandler : ICommandHandler<DepartmentDto, UpdateDepartmentCommand>
{
    private readonly ILogger<UpdateDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IValidator<UpdateDepartmentRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public UpdateDepartmentHandler(ILogger<UpdateDepartmentHandler> logger,
                                   IDepartmentsRepository departmentsRepository,
                                   IValidator<UpdateDepartmentRequest> validator,
                                   ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<DepartmentDto, Failure>> HandleAsync(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validatiorResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            return validatiorResult.ToErrors();
        }

        if (command.Id == Guid.Empty)
        {
            return Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error").ToFailure();
        }

        var department = await _departmentsRepository.GetByAsync(d => d.Id == command.Id && !d.IsDeleted, cancellationToken);

        if (department.IsFailure)
            return department.Error;


        if (command.Request.Name != null)
        {
            var name = Name.Create(command.Request.Name);

            if ((await _departmentsRepository.GetByAsync(d => d.Name == name.Value, cancellationToken)).IsSuccess)
            {
                return Errors.DepartmentErrors.Conflict(name.ToString()).ToFailure();
            }

            department.Value.SetName(name.Value);
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("The department with ID {Id} was updated.", command.Id);

        return new DepartmentDto(command.Id,
            department.Value.Name.ToString(),
            department.Value.Slug.ToString(),
            department.Value.Path.ToString(),
            department.Value.ParentId);
    }
}
