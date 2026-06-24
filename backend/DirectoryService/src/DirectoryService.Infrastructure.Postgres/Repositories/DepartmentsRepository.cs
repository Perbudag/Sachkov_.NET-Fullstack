using DirectoryService.Core.Services.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class DepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(AppDbContext context, ILogger<DepartmentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Departments.AddAsync(department, cancellationToken);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create department with id: {Id}", department.Id);
            throw;
        }
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _context.Departments.AnyAsync(d => d.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsChildWithSlugAsync(Department parent, Slug slug, CancellationToken cancellationToken)
    {
        return await _context.Departments.AnyAsync(d => d.ParentId == parent.Id && d.Slug == slug, cancellationToken);
    }

    public async Task<Department?> GetByIdAsync(Guid? departmentId, CancellationToken cancellationToken)
    {
        return await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);
    }

    public async Task AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocations = locations.Select(l => DepartmentLocation.Create(department.Id, l.Id));

        await _context.DepartmentLocations.AddRangeAsync(departmentLocations, cancellationToken);
    }

    public async Task RemoveLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocations = await _context.DepartmentLocations.Where(dl => dl.DepartmentId == department.Id &&
            locations.Select(l => l.Id).Contains(dl.LocationId))
            .ToListAsync(cancellationToken);

        _context.DepartmentLocations.RemoveRange(departmentLocations);
    }
    public async Task<bool> LocationExistsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        return await _context.DepartmentLocations.Where(dl => dl.DepartmentId == department.Id && locations.Select(l => l.Id).Contains(dl.LocationId)).AnyAsync(cancellationToken);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }


}
