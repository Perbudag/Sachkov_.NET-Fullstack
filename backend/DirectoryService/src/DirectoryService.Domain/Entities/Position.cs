using DirectoryService.Domain.Relationships;
using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Position
{
    private readonly List<DepartmentPosition> _departments = [];

    private Position(Name name)
    {
        Id = Guid.CreateVersion7();

        Name = name;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Position() { }

    public Guid Id { get; }
    public Name Name { get; private set; } = null!;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<DepartmentPosition> Departments => _departments;


    public static Position Create(Name name)
    {
        return new Position(name);
    }

    public void SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDepartment(Department department)
    {
        _departments.Add(DepartmentPosition.Create(department.Id, Id));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemoveDepartment(Department department)
    {
        var findedDepartment = _departments.FirstOrDefault(d => d.Id == department.Id);

        if (findedDepartment != null)
        {
            UpdatedAt = DateTime.UtcNow;
            return _departments.Remove(findedDepartment);
        }

        return false;
    }
}
