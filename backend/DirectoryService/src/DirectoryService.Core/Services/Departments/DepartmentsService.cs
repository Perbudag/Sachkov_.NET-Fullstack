using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Presenters;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Core.Services.Departments;

internal class DepartmentsService : IDepartmentsService
{
    private readonly ILogger<DepartmentsService> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentRequest> _createValidator;
    private readonly IValidator<UpdateDepartmentRequest> _updateValidator;

    public DepartmentsService(ILogger<DepartmentsService> logger,
                              IDepartmentsRepository departmentsRepository,
                              ILocationsRepository locationsRepository,
                              IValidator<CreateDepartmentRequest> createValidator,
                              IValidator<UpdateDepartmentRequest> updateValidator)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _createValidator = createValidator;
        _locationsRepository = locationsRepository;
        _updateValidator = updateValidator;
    }


    public async Task<Result<Guid, Failure>> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validatiorResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            return validatiorResult.ToErrors(Errors.DepartmentErrors.ValidationError);
        }

        Department? parent = null;
        List<Location>? locations = null;

        if (request.ParentId != null)
        {
            var parentResult = await _departmentsRepository.GetByIdAsync(request.ParentId.Value, cancellationToken);

            if (parentResult.IsFailure)
            {
                return Errors.DepartmentErrors.NotFoudParent().ToFailure();
            }

            parent = parentResult.Value;
        }

        if (request.LocationIds.Any())
        {
            var locationsResult = await _locationsRepository.GetByIdsAsync(request.LocationIds, cancellationToken);

            if (locationsResult.IsFailure)
                return locationsResult.Error;

            locations = locationsResult.Value.ToList();

            var notFoundLocationIds = request.LocationIds.Where(lId => !locations.Select(l => l.Id).Contains(lId));

            if (notFoundLocationIds.Any())
            {
                return Errors.LocationErrors.NotFoudMany(notFoundLocationIds).ToFailure();
            }
        }

        var errors = new List<Error>();

        var name = Name.Create(request.Name);
        var slug = Slug.Create(request.Slug);

        if (name.IsFailure)
            errors.AddRange(name.Error);

        if (slug.IsFailure)
            errors.AddRange(slug.Error);

        if (errors.Count > 0)
            return new Failure(errors);


        var department = Department.Create(name.Value, slug.Value, parent);

        if (department.IsFailure)
            return department.Error;

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
        _logger.LogInformation("Department created with name \"{Name}\".", name);

        return department.Value.Id;
    }

    public async Task<Result<DepartmentResponse, Failure>> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validatiorResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            return validatiorResult.ToErrors(Errors.DepartmentErrors.ValidationError);
        }

        if (id == Guid.Empty)
        {
            return Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error").ToFailure();
        }

        var department = await _departmentsRepository.GetByIdAsync(id, cancellationToken);

        if (department.IsFailure)
            return department.Error;


        if (request.Name != null)
        {
            var name = Name.Create(request.Name);

            if (name.IsFailure)
                return name.Error;

            if ((await _departmentsRepository.GetByNameAsync(name.Value, cancellationToken)).IsSuccess)
            {
                return Errors.DepartmentErrors.Conflict(name.ToString()).ToFailure();
            }

            department.Value.SetName(name.Value);
        }

        await _departmentsRepository.SaveAsync(cancellationToken);

        return new DepartmentResponse(id,
            department.Value.Name.ToString(),
            department.Value.Slug.ToString(),
            department.Value.Path.ToString(),
            department.Value.ParentId);
    }

    public async Task<UnitResult<Failure>> AddLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (departmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (locationId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error"));

        if (errors.Count > 0)
            return new Failure(errors);

        var departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

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

        await _departmentsRepository.SaveAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<UnitResult<Failure>> RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {

        var errors = new List<Error>();

        if (departmentId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error"));

        if (locationId == Guid.Empty)
            errors.Add(Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error"));


        if (errors.Count > 0)
            return new Failure(errors);


        var departmentResult = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var locationResult = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

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

        var result = await _departmentsRepository.RemoveLocationsAsync(departmentResult.Value, [locationResult.Value], cancellationToken);

        if (result.IsFailure)
            return result.Error;

        await _departmentsRepository.SaveAsync(cancellationToken);

        return UnitResult.Success<Failure>();
    }
}
