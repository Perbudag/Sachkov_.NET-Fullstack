using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Entities;

public class Position
{
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
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }


    public static Result<Position, Failure> Create(Name name)
    {
        return new Position(name);
    }

    public UnitResult<Failure> SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
