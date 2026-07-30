using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.AddLocation;

internal class AddLocationInDepartmentHandler : ICommandHandler<AddLocationInDepartmentCommand>
{
    private readonly ILogger<AddLocationInDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;

    public AddLocationInDepartmentHandler(ILogger<AddLocationInDepartmentHandler> logger,
                                          IDepartmentsRepository departmentsRepository,
                                          ILocationsRepository locationsRepository,
                                          ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(AddLocationInDepartmentCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (command.DepartmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (command.LocationId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error"));

        if (errors.Count > 0)
            return new Failure(errors);

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        var locationResult = await _locationsRepository.GetByIdAsync(command.LocationId, cancellationToken);

        if (departmentResult.IsFailure)
        {
            errors.Add(Errors.DepartmentErrors.NotFoud());
        }
        if (locationResult.IsFailure)
        {
            errors.Add(Errors.DepartmentErrors.LocationNotFound());
        }

        if (errors.Count > 0)
            return new Failure(errors);

        var result = await _departmentsRepository.AddLocationsAsync(departmentResult.Value, [locationResult.Value], cancellationToken);

        if (result.IsFailure)
            return result.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("A location with ID {LocationId} has been added to the department with ID {DepartmentId}.",
            command.LocationId, command.DepartmentId);

        return UnitResult.Success<Failure>();
    }
}
