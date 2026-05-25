using DirectoryService.Domain.Relationships;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];

    private Location(Name name, Address address)
    {
        Id = Guid.CreateVersion7();

        Name = name;
        Address = address;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Location() {}

    public Guid Id { get; }
    public Name Name { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentLocation> Departments => _departments;


    public static Location Create(Name name, Address address)
    {
        return new Location(name, address);
    }

    public void SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDepartment(Department department)
    {
        _departments.Add(DepartmentLocation.Create(department.Id, Id));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemoveDepartment(Department department)
    {
        var findedDepartment = _departments.FirstOrDefault(d => d.DepartmentId == department.Id);

        if(findedDepartment != null)
        {
            UpdatedAt = DateTime.UtcNow;
            return _departments.Remove(findedDepartment);
        }

        return false;
    }
}
