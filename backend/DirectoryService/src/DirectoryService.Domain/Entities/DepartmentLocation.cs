using CSharpFunctionalExtensions;
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
            errors.Add(Error.Validation("departmentId не должен быть пустым.", "department.location.validation.error", nameof(departmentId)));
        }

        if (locationId == Guid.Empty)
        {
            errors.Add(Error.Validation("locationId не должен быть пустым.", "department.location.validation.error", nameof(locationId)));
        }

        if (errors.Count > 0)
            return new Failure(errors);

        return new DepartmentLocation(departmentId, locationId);
    }
}
