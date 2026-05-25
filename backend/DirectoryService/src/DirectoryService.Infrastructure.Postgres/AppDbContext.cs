using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class AppDbContext : DbContext
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
    public DbSet<DepartmentPosition> DepartmentsPositions { get; set; }


    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
