using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
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

    public CreateDepartmentHandler(ILogger<CreateDepartmentHandler> logger,
                                   IDepartmentsRepository departmentsRepository,
                                   ILocationsRepository locationsRepository,
                                   IValidator<CreateDepartmentRequest> validator)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
    }

    public async Task<Result<Guid, Failure>> HandleAsync(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validatiorResult = await _validator.ValidateAsync(command.Request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            return validatiorResult.ToErrors();
        }

        Department? parent = null;
        List<Location>? locations = null;

        if (command.Request.ParentId != null)
        {
            var parentResult = await _departmentsRepository.GetByIdAsync(command.Request.ParentId.Value, cancellationToken);

            if (parentResult.IsFailure)
            {
                return Errors.DepartmentErrors.NotFoudParent().ToFailure();
            }

            parent = parentResult.Value;
        }

        if (command.Request.LocationIds.Any())
        {
            var locationsResult = await _locationsRepository.GetByIdsAsync(command.Request.LocationIds, cancellationToken);

            if (locationsResult.IsFailure)
                return locationsResult.Error;

            locations = locationsResult.Value.ToList();

            var notFoundLocationIds = command.Request.LocationIds.Where(lId => !locations.Select(l => l.Id).Contains(lId));

            if (notFoundLocationIds.Any())
            {
                return Errors.LocationErrors.NotFoudMany(notFoundLocationIds).ToFailure();
            }
        }

        var name = Name.Create(command.Request.Name);
        var slug = Slug.Create(command.Request.Slug);

        var department = Department.Create(name.Value, slug.Value, parent);

        var addResult = await _departmentsRepository.AddAsync(department.Value, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error;

        if (locations != null && locations.Count > 0)
        {
            var addLocationResult = await _departmentsRepository.AddLocationsAsync(department.Value, locations, cancellationToken);

            if (addLocationResult.IsFailure)
                return addLocationResult.Error;
        }

        await _departmentsRepository.SaveAsync(cancellationToken);
        _logger.LogInformation("Department created with name \"{Name}\".", name.Value);

        return department.Value.Id;
    }
}
