using CSharpFunctionalExtensions;
using DirectoryService.Domain.Abstracts;
using DirectoryService.Domain.ValueObjects;
using Shared;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Domain.Entities;


public class Department : ISoftDeletable
{
    public Guid Id { get; }
    public Name Name { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public Path Path { get; private set; } = null!;
    public int Depth { get; private set; }
    public Guid? ParentId { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private Department(Name name, Slug slug, Department? parent)
    {
        Id = Guid.CreateVersion7();

        Name = name;
        Slug = slug;
        Path = Path.Create([.. parent?.Path.Slugs ?? [], slug]).Value;
        Depth = parent != null ? parent.Depth + 1 : 0;
        ParentId = parent?.Id;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core
    private Department() { }

    public static Result<Department, Failure> Create(Name name, Slug slug, Department? parent)
    {
        return new Department(name, slug, parent);
    }


    public UnitResult<Failure> SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> SetSlug(Slug slug)
    {
        Slug = slug;
        Path = Path.Create([.. Path.Slugs.SkipLast(1) ?? [], slug]).Value;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> SetParent(Department? parent)
    {
        if (this == parent)
        {
            var error = Error.Conflict("Department не может быть родителем" +
                "для самого себя", "department.is.conflict");

            return error.ToFailure();
        }
        if (parent != null 
            && (parent.Path.ToString().StartsWith(this.Path.ToString() + ".", StringComparison.Ordinal) 
                || string.Equals(this.Path.ToString(), parent.Path.ToString(), StringComparison.Ordinal)))
        {
            var error = Error.Conflict("Department не может быть потомком нового родителя", "department.is.conflict");
            return error.ToFailure();
        }

        ParentId = parent?.Id;
        Path = Path.Create([.. parent?.Path.Slugs ?? [], Slug]).Value;
        Depth = parent != null ? parent.Depth + 1 : 0;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}