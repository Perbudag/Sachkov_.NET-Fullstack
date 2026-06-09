using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

internal class DepartmentsService : IDepartmentsService
{
    private readonly ILogger<DepartmentsService> _logger;
    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentRequest> _validator;

    public DepartmentsService(ILogger<DepartmentsService> logger,
                              IDepartmentsRepository departmentsRepository,
                              ILocationsRepository locationsRepository,
                              IValidator<CreateDepartmentRequest> validator)
    {
        _logger = logger;
        _departmentsRepository = departmentsRepository;
        _validator = validator;
        _locationsRepository = locationsRepository;
    }

    public async Task<Guid> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var validatiorResult = await _validator.ValidateAsync(request, cancellationToken);
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

        var department = Department.Create(name, slug, parent);

        await _departmentsRepository.AddAsync(department, cancellationToken);

        if (locations != null && locations.Count > 0)
        {
            await _departmentsRepository.AddLocations(department, locations, cancellationToken);
        }

        await _departmentsRepository.SaveAsync(cancellationToken);
        _logger.LogInformation("Department created with name \"{Name}\".", name);

        return department.Id;
    }
}
