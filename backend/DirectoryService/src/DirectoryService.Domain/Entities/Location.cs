using DirectoryService.Domain.ValueObjects;

namespace DirectoryService.Domain.Entities;

public class Location
{
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
}
