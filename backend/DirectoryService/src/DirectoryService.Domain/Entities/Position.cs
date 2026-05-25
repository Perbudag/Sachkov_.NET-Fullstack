using DirectoryService.Domain.ValueObjects;

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
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }


    public static Position Create(Name name)
    {
        return new Position(name);
    }

    public void SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}
