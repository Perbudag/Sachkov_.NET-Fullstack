using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class AppDbContext : DbContext, IReadDbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
    public DbSet<DepartmentPosition> DepartmentsPositions { get; set; }


    public IQueryable<Department> DepartmentsRead => Departments.Where(d => !d.IsDeleted).AsNoTracking();
    public IQueryable<Location> LocationsRead => Locations.Where(l => !l.IsDeleted).AsNoTracking();
    public IQueryable<Position> PositionsRead => Positions.Where(p => !p.IsDeleted).AsNoTracking();
    public IQueryable<DepartmentLocation> DepartmentLocationsRead => DepartmentLocations.AsQueryable().AsNoTracking();
    public IQueryable<DepartmentPosition> DepartmentsPositionsRead => DepartmentsPositions.AsQueryable().AsNoTracking();


    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
