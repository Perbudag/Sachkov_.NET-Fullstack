using DirectoryService.Domain.Entities;

namespace DirectoryService.Core.Abstractions.Database;

public interface IReadDbContext
{
    IQueryable<Department> DepartmentsRead { get; }
    IQueryable<Location> LocationsRead { get; }
    IQueryable<Position> PositionsRead { get; }
    IQueryable<DepartmentLocation> DepartmentLocationsRead { get; }
    IQueryable<DepartmentPosition> DepartmentsPositionsRead { get; }
}