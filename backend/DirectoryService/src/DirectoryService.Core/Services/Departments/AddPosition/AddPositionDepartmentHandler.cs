using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Services.Positions;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.AddPosition;

internal class AddPositionDepartmentHandler : ICommandHandler<AddPositionDepartmentCommand>
{
    private readonly ILogger<AddPositionDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly IPositionsRepository _positionsRepository;
    private readonly ITransactionManager _transactionManager;

    public AddPositionDepartmentHandler(ILogger<AddPositionDepartmentHandler> logger,
                                        IDepartmentsRepository departmentsRepository,
                                        IPositionsRepository positionsRepository,
                                        ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _positionsRepository = positionsRepository;
        _transactionManager = transactionManager;
    }

    public async Task<UnitResult<Failure>> HandleAsync(AddPositionDepartmentCommand command, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (command.DepartmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (command.PositionId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("PositionId", "departments.validation.error"));

        if (errors.Count > 0)
            return new Failure(errors);

        var departmentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.DepartmentId, cancellationToken);
        var positionResult = await _positionsRepository.GetByAsync(p => p.Id == command.PositionId, cancellationToken);

        if (departmentResult.IsFailure)
        {
            errors.AddRange(departmentResult.Error);
        }
        if (positionResult.IsFailure)
        {
            errors.AddRange(positionResult.Error);
        }

        if (errors.Count > 0)
            return new Failure(errors);

        var result = await _departmentsRepository.AddPositionsAsync(departmentResult.Value, [positionResult.Value], cancellationToken);

        if (result.IsFailure)
            return result.Error;

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("A position with ID {PositionId} has been added to the department with ID {DepartmentId}.",
            command.PositionId, command.DepartmentId);

        return UnitResult.Success<Failure>();

    }
}
