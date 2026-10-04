using CSharpFunctionalExtensions;
using DirectoryService.Domain.Errors;
using Shared;

namespace DirectoryService.Domain.Entities;

public sealed class DepartmentLocation
{
    private DepartmentLocation(Guid departmentId, Guid locationId)
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;

        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation() { }


    public Guid Id { get; }

    public Guid DepartmentId { get; }
    public Guid LocationId { get; }

    public DateTime CreatedAt { get; }


    public static Result<DepartmentLocation, Failure> Create(Guid departmentId, Guid locationId)
    {
        var errors = new List<Error>();

        if (departmentId == Guid.Empty)
        {
            errors.Add(SharedErrors.IsRequired(nameof(departmentId), "department.location.validation.error"));
        }

        if (locationId == Guid.Empty)
        {
            errors.Add(SharedErrors.IsRequired(nameof(locationId), "department.location.validation.error"));
        }

        if (errors.Count > 0)
            return new Failure(errors);

        return new DepartmentLocation(departmentId, locationId);
    }
}
