using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects;
using Shared;

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
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }


    public static Result<Location, Failure> Create(Name name, Address address)
    {
        return new Location(name, address);
    }

    public UnitResult<Failure> SetName(Name name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> SetAddress(Address address)
    {
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Failure>();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
