using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.RemoveLocation;

internal class RemoveLocationInDepartmentHandler : ICommandHandler<RemoveLocationInDepartmentCommand>
{
    private readonly ILogger<RemoveLocationInDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly ITransactionManager _transactionManager;

    public RemoveLocationInDepartmentHandler(ILogger<RemoveLocationInDepartmentHandler> logger,
                                             IDepartmentsRepository departmentsRepository,
                                             ILocationsRepository locationsRepository,
                                             ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(RemoveLocationInDepartmentCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (command.DepartmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (command.LocationId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error"));


        if (errors.Count > 0)
            return new Failure(errors);


        var departmentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.DepartmentId, cancellationToken);
        var locationResult = await _locationsRepository.GetByAsync(l => l.Id == command.LocationId, cancellationToken);

        if (departmentResult.IsFailure)
        {
            errors.AddRange(departmentResult.Error);
        }
        if (locationResult.IsFailure)
        {
            errors.AddRange(locationResult.Error);
        }

        if (errors.Count > 0)
            return new Failure(errors);

        var result = await _departmentsRepository.RemoveLocationsAsync(departmentResult.Value, [locationResult.Value], cancellationToken);

        if (result.IsFailure)
            return result.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("The location with ID {LocationId} has been removed from the department with ID {DepartmentId}.",
            command.LocationId, command.DepartmentId);

        return UnitResult.Success<Failure>();
    }
}
