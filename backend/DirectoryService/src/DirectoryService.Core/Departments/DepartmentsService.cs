using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Presenters;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

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
            throw new ValidationException(validatiorResult.Errors);
        }

        Department? parent = null;
        List<Location>? locations = null;

        if (request.ParentId != null)
        {
            parent = await _departmentsRepository.GetByIdAsync(request.ParentId, cancellationToken);

            if (parent == null)
            {
                throw new ValidationException("Department with this id not found");
            }
        }

        if (request.LocationIds.Any())
        {
            locations = (await _locationsRepository.GetByIdsAsync(request.LocationIds, cancellationToken)).ToList();

            var notFoundLocationIds = request.LocationIds.Where(lId => !locations.Select(l => l.Id).Contains(lId));

            if (notFoundLocationIds.Any())
            {
                throw new ValidationException("Locations with this ids not found: " + string.Join(", ", notFoundLocationIds));
            }
        }

        var name = Name.Create(request.Name);

        if (await _departmentsRepository.ExistsByNameAsync(name, cancellationToken))
        {
            throw new ValidationException($"A department named \"{name}\" already exists");
        }

        var slug = Slug.Create(request.Slug);

        if (parent != null && await _departmentsRepository.ExistsChildWithSlugAsync(parent, slug, cancellationToken))
        {
            throw new ValidationException($"The department with id \"{parent.Id}\" " +
                $"already has a child element with Slug \"{slug}\"");
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
            throw new ValidationException(validatiorResult.Errors);
        }

        if(id == Guid.Empty)
        {
            throw new ValidationException("Id is required");
        }

        var department = await _departmentsRepository.GetByIdAsync(id, cancellationToken);

        if (department == null)
        {
            throw new ValidationException("Department with this id not found");
        }


        if(request.Name != null)
        {
            var name = Name.Create(request.Name);

            if (await _departmentsRepository.ExistsByNameAsync(name, cancellationToken))
            {
                throw new ValidationException($"A department named \"{name}\" already exists");
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
        if(departmentId == Guid.Empty)
        {
            throw new ValidationException("DepartmentId is required");
        }
        if (locationId == Guid.Empty)
        {
            throw new ValidationException("LocationId is required");
        }

        var department = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var location = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

        if (department == null)
        {
            throw new ValidationException($"Department with id \"{departmentId}\" not found");
        }
        if (location == null)
        {
            throw new ValidationException($"Location with id \"{locationId}\" not found");
        }

        await _departmentsRepository.AddLocationsAsync(department, [location], cancellationToken);
        await _departmentsRepository.SaveAsync(cancellationToken);
    }

    public async Task RemoveLocationAsync(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        if (departmentId == Guid.Empty)
        {
            throw new ValidationException("DepartmentId is required");
        }
        if (locationId == Guid.Empty)
        {
            throw new ValidationException("LocationId is required");
        }

        var department = await _departmentsRepository.GetByIdAsync(departmentId, cancellationToken);
        var location = await _locationsRepository.GetByIdAsync(locationId, cancellationToken);

        if (department == null)
        {
            throw new ValidationException($"Department with id \"{departmentId}\" not found");
        }
        if (location == null)
        {
            throw new ValidationException($"Location with id \"{locationId}\" not found");
        }

        await _departmentsRepository.RemoveLocationsAsync(department, [location], cancellationToken);
        await _departmentsRepository.SaveAsync(cancellationToken);
    }
}
