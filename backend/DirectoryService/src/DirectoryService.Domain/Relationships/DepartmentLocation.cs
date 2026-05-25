namespace DirectoryService.Domain.Relationships;

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


    public static DepartmentLocation Create(Guid departmentId, Guid locationId)
    {
        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException("departmentId не должен быть пустым.", nameof(departmentId));
        }

        if (locationId == Guid.Empty)
        {
            throw new ArgumentException("locationId не должен быть пустым.", nameof(locationId));
        }

        return new DepartmentLocation(departmentId, locationId);
    }
}
