using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Infrastructure.Postgres;

namespace DirectoryService.IntegrationTests.TestData;

internal static class DbTestData
{
    public static async Task<Guid> CreateLocationAsync(
        AppDbContext dbContext,
        string name,
        CancellationToken cancellationToken)
    {
        var locationName = Name.Create(name).Value;
        var address = Address.Create(
            "103132",
            "Россия",
            "Москва",
            "Москва",
            "Кремлевская набережная",
            "1",
            null).Value;

        var location = Location.Create(locationName, address).Value;

        dbContext.Locations.Add(location);
        await dbContext.SaveChangesAsync(cancellationToken);

        return location.Id;
    }

    public static async Task<Guid> CreateDepartmentAsync(
        AppDbContext dbContext,
        string name,
        string slug,
        Guid? parentId,
        CancellationToken cancellationToken)
    {
        Department? parent = null;
        if (parentId.HasValue)
        {
            parent = await dbContext.Departments.FindAsync([parentId.Value], cancellationToken)
                ?? throw new InvalidOperationException($"Parent department '{parentId}' was not found.");
        }

        var department = Department.Create(
            Name.Create(name).Value,
            Slug.Create(slug).Value,
            parent).Value;

        dbContext.Departments.Add(department);
        await dbContext.SaveChangesAsync(cancellationToken);

        return department.Id;
    }

    public static async Task<Guid> CreatePositionAsync(
        AppDbContext dbContext,
        string name,
        CancellationToken cancellationToken)
    {
        var position = Position.Create(Name.Create(name).Value).Value;

        dbContext.Positions.Add(position);
        await dbContext.SaveChangesAsync(cancellationToken);

        return position.Id;
    }

    public static async Task CreateDepartmentLocationAsync(
        AppDbContext dbContext,
        Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var relation = DepartmentLocation.Create(departmentId, locationId).Value;

        dbContext.DepartmentLocations.Add(relation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static async Task CreateDepartmentPositionAsync(
        AppDbContext dbContext,
        Guid departmentId,
        Guid positionId,
        CancellationToken cancellationToken)
    {
        var relation = DepartmentPosition.Create(departmentId, positionId).Value;

        dbContext.DepartmentsPositions.Add(relation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
