using DirectoryService.Domain.Relationships;
using DirectoryService.Domain.ValueObjects;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Domain.Entities;


public class Department
{
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    private Department(Name name, Slug slug, Department? parent)
    {
        Id = Guid.CreateVersion7();

        Name = name;
        Slug = slug;
        Path = Path.Create([.. parent?.Path.Slugs ?? [], slug]);
        ParentId = parent?.Id;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Department() {}

    public Guid Id { get; }
    public Name Name { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public Path Path { get; private set; } = null!;
    public Guid? ParentId { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<DepartmentLocation> Locations => _locations;
    public IReadOnlyList<DepartmentPosition> Positions => _positions;


    public static Department Create(Name name, Slug slug, Department? parent)
    {
        return new Department(name, slug, parent);
    }


    public void SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSlug(Slug slug)
    {
        Slug = slug;
        Path = Path.Create([.. Path.Slugs.SkipLast(1) ?? [], slug]);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Department parent)
    {
        if (this == parent)
        {
            throw new ArgumentException("Department не может быть родителем" +
                "для самого себя", nameof(parent));
        }

        ParentId = parent.Id;
        Path = Path.Create([.. parent?.Path.Slugs ?? [], Slug]);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLocation(Location location)
    {
        _locations.Add(DepartmentLocation.Create(Id, location.Id));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemoveLocation(Location location)
    {
        var findedLocation = _locations.FirstOrDefault(l => l.LocationId == location.Id);

        if (findedLocation != null)
        {
            UpdatedAt = DateTime.UtcNow;
            return _locations.Remove(findedLocation);
        }

        return false;
    }

    public void AddPosition(Position position)
    {
        _positions.Add(DepartmentPosition.Create(Id, position.Id));
        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemovePosition(Position position)
    {
        var findedPosition = _positions.FirstOrDefault(p => p.PositionId == position.Id);

        if (findedPosition != null)
        {
            UpdatedAt = DateTime.UtcNow;
            return _positions.Remove(findedPosition);
        }

        return false;
    }
}