using DirectoryService.Core.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class EFDepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EFDepartmentsRepository> _logger;

    public EFDepartmentsRepository(AppDbContext context, ILogger<EFDepartmentsRepository> logger)
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

    public async Task AddLocations(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocations = locations.Select(l => DepartmentLocation.Create(department.Id, l.Id));

        await _context.DepartmentLocations.AddRangeAsync(departmentLocations, cancellationToken);
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

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
