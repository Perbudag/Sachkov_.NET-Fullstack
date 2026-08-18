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


    public IQueryable<Department> DepartmentsRead => Departments.AsNoTracking();
    public IQueryable<Location> LocationsRead => Locations.AsNoTracking();
    public IQueryable<Position> PositionsRead => Positions.AsNoTracking();
    public IQueryable<DepartmentLocation> DepartmentLocationsRead => DepartmentLocations.AsNoTracking();
    public IQueryable<DepartmentPosition> DepartmentsPositionsRead => DepartmentsPositions.AsNoTracking();


    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<Location>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Position>().HasQueryFilter(p => !p.IsDeleted);
    }
}
