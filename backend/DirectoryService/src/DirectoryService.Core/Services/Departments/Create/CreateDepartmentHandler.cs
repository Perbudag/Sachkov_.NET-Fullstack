using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments.Create;

internal class CreateDepartmentHandler : ICommandHandler<Guid, CreateDepartmentCommand>
{
    private readonly ILogger<CreateDepartmentHandler> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentRequest> _validator;
    private readonly ITransactionManager _transactionManager;

    public CreateDepartmentHandler(ILogger<CreateDepartmentHandler> logger,
                                   IDepartmentsRepository departmentsRepository,
                                   ILocationsRepository locationsRepository,
                                   IValidator<CreateDepartmentRequest> validator,
                                   ITransactionManager transactionManager)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _transactionManager = transactionManager;
    }

    public async Task<Result<Guid, Failure>> HandleAsync(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validateResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validateResult.IsValid)
        {
            return validateResult.ToErrors();
        }

        Department? parent = null;
        List<Location>? locations = null;

        if (command.Request.ParentId != null)
        {
            var parentResult = await _departmentsRepository.GetByAsync(d => d.Id == command.Request.ParentId, cancellationToken);

            if (parentResult.IsFailure)
            {
                return Errors.DepartmentErrors.NotFoudParent().ToFailure();
            }

            parent = parentResult.Value;
        }

        if (command.Request.LocationIds.Any())
        {
            locations = await _locationsRepository
                .GetByAsyncEnum(l => command.Request.LocationIds.Contains(l.Id))
                .ToListAsync(cancellationToken);

            var errors = command.Request.LocationIds
                .Where(lId => !locations
                .Select(l => l.Id)
                .Contains(lId))
                .Select(lId => Errors.LocationErrors.NotFoud(lId));

            if (errors.Any())
            {
                return new Failure(errors);
            }
        }

        var name = Name.Create(command.Request.Name).Value;
        var slug = Slug.Create(command.Request.Slug).Value;

        var department = Department.Create(name, slug, parent).Value;

        var addResult = await _departmentsRepository.AddAsync(department, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error;

        if (locations != null && locations.Count > 0)
        {
            var addLocationResult = await _departmentsRepository.AddLocationsAsync(department, locations, cancellationToken);

            if (addLocationResult.IsFailure)
                return addLocationResult.Error;
        }

        var saveResult = await _transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
            return saveResult.Error.ToFailure();

        _logger.LogInformation("Department created with name \"{Name}\".", name.Value);

        return department.Id;
    }
}
