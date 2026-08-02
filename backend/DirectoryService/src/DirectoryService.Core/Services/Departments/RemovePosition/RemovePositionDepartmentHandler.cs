using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Departments.RemoveLocation;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Services.Positions;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.RemovePosition;

internal class RemovePositionDepartmentHandler : ICommandHandler<RemovePositionDepartmentCommand>
{
    private readonly ILogger<RemovePositionDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;

    public RemovePositionDepartmentHandler(ILogger<RemovePositionDepartmentHandler> logger,
                                           IDepartmentsRepository departmentsRepository,
                                           IPositionsRepository positionsRepository,
                                           ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(RemovePositionDepartmentCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (command.DepartmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (command.PositionId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("PositionId", "departments.validation.error"));

        if (errors.Count > 0)
            return new Failure(errors);

        var departmentResult = await _departmentsRepository.GetByIdAsync(command.DepartmentId, cancellationToken);
        var positionResult = await _positionsRepository.GetByIdAsync(command.PositionId, cancellationToken);

        if (departmentResult.IsFailure)
        {
            errors.Add(Errors.DepartmentErrors.NotFoud());
        }
        if (positionResult.IsFailure)
        {
            errors.Add(Errors.DepartmentErrors.PositionNotFound());
        }

        if (errors.Count > 0)
            return new Failure(errors);

        var result = await _departmentsRepository.RemovePositionsAsync(departmentResult.Value, [positionResult.Value], cancellationToken);

        if (result.IsFailure)
            return result.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("The position with ID {PositionId} has been removed from the department with ID {DepartmentId}.",
            command.PositionId, command.DepartmentId);

        return UnitResult.Success<Failure>();
    }
}
