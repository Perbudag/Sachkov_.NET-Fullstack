using DirectoryService.Domain.ValueObjects;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Domain.Entities;


public class Department
{
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
}