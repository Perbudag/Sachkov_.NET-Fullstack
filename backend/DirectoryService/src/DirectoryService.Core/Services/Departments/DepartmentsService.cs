using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Extensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Departments.Fails.Exceptions;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Services.Locations.Fails.Exceptions;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Presenters;
using FluentValidation;
using Microsoft.Extensions.Logging;

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


    public async Task<Guid> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validatiorResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            var errors = validatiorResult.ToErrors(Errors.DepartmentErrors.ValidationError);

            throw new DepartmentsValidationException(errors);
        }

        Department? parent = null;
        List<Location>? locations = null;

        if (request.ParentId != null)
        {
            parent = await _departmentsRepository.GetByIdAsync(request.ParentId, cancellationToken);

            if (parent == null)
            {
                var error = Errors.DepartmentErrors.NotFoud();

                throw new DepartmentsNotFoundException(error);
            }
        }

        if (request.LocationIds.Any())
        {
            locations = (await _locationsRepository.GetByIdsAsync(request.LocationIds, cancellationToken)).ToList();

            var notFoundLocationIds = request.LocationIds.Where(lId => !locations.Select(l => l.Id).Contains(lId));

            if (notFoundLocationIds.Any())
            {
                var error = Errors.LocationErrors.NotFoudMany(notFoundLocationIds);

                throw new LocationsNotFoundException(error);
            }
        }

        var name = Name.Create(request.Name);

        if (await _departmentsRepository.ExistsByNameAsync(name, cancellationToken))
        {
            var error = Errors.DepartmentErrors.Conflict(name.ToString());

            throw new DepartmentsConflictException(error);
        }

        var slug = Slug.Create(request.Slug);

        if (parent != null && await _departmentsRepository.ExistsChildWithSlugAsync(parent, slug, cancellationToken))
        {
            var error = Errors.DepartmentErrors.SlugConflict(parent.Id, slug.ToString());

            throw new DepartmentsConflictException(error);
        }

        var department = Department.Create(name, slug, parent);

        await _departmentsRepository.AddAsync(department, cancellationToken);

        if (locations != null && locations.Count > 0)
        {
            await _departmentsRepository.AddLocationsAsync(department, locations, cancellationToken);
        }

        await _departmentsRepository.SaveAsync(cancellationToken);
        _logger.LogInformation("Department created with name \"{Name}\".", name);

        return department.Id;
    }

    public async Task<DepartmentResponse> UpdateAsync(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validatiorResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validatiorResult.IsValid)
        {
            var errors = validatiorResult.ToErrors(Errors.DepartmentErrors.ValidationError);

            throw new BadRequestException(errors);
        }

        if (id == Guid.Empty)
        {
            var error = Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error");

            throw new DepartmentsValidationException(error);
        }

        var department = await _departmentsRepository.GetByIdAsync(id, cancellationToken);

        if (department == null)
        {
            var error = Errors.DepartmentErrors.NotFoud();

            throw new DepartmentsNotFoundException(error);
        }


        if (request.Name != null)
        {
            var name = Name.Create(request.Name);

            if (await _departmentsRepository.ExistsByNameAsync(name, cancellationToken))
            {
                var error = Errors.DepartmentErrors.Conflict(name.ToString());

                throw new DepartmentsConflictException(error);
            }

            department.SetName(name);
        }

        await _departmentsRepository.SaveAsync(cancellationToken);

        return new DepartmentResponse(id,
            department.Name.ToString(),
            department.Slug.ToString(),
            department.Path.ToString(),
            department.ParentId);
    }

    public async Task AddLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        if (departmentId == Guid.Empty)
        {
            var error = Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error");

            throw new DepartmentsValidationException(error);
        }
        if (locationId == Guid.Empty)
        {
            var error = Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error");

            throw new DepartmentsValidationException(error);
        }

        var department = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var location = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

        if (department == null)
        {
            var error = Errors.DepartmentErrors.NotFoud();

            throw new DepartmentsNotFoundException(error);
        }
        if (location == null)
        {
            var error = Errors.DepartmentErrors.LocationNotFoud();

            throw new LocationsNotFoundException(error);
        }

        if(await _departmentsRepository.LocationExistsAsync(department, [location], cancellationToken))
        {
            var error = Errors.DepartmentErrors.LocationConflict();

            throw new DepartmentsConflictException(error);
        }

        await _departmentsRepository.AddLocationsAsync(department, [location], cancellationToken);
        await _departmentsRepository.SaveAsync(cancellationToken);
    }

    public async Task RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        if (departmentId == Guid.Empty)
        {
            var error = Errors.SharedErrors.IsRequired("DepartmentId", "departments.validation.error");

            throw new DepartmentsValidationException(error);
        }
        if (locationId == Guid.Empty)
        {
            var error = Errors.SharedErrors.IsRequired("LocationId", "departments.validation.error");

            throw new DepartmentsValidationException(error);
        }

        var department = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var location = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

        if (department == null)
        {
            var error = Errors.DepartmentErrors.NotFoud();

            throw new DepartmentsNotFoundException(error);
        }
        if (location == null)
        {
            var error = Errors.DepartmentErrors.LocationNotFoud();

            throw new LocationsNotFoundException(error);
        }

        if (!await _departmentsRepository.LocationExistsAsync(department, [location], cancellationToken))
        {
            var error = Errors.DepartmentErrors.LocationNotFoud();

            throw new LocationsNotFoundException(error);
        }

        await _departmentsRepository.RemoveLocationsAsync(department, [location], cancellationToken);
        await _departmentsRepository.SaveAsync(cancellationToken);
    }
}
