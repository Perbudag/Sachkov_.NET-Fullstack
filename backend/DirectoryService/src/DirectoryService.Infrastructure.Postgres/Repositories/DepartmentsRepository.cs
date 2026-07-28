using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Xml.Linq;

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

    public async Task<UnitResult<Failure>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();

        if (await _context.Departments.AnyAsync(d => d.Name == department.Name, cancellationToken))
        {
            errors.Add(Errors.DepartmentErrors.Conflict(department.Name.ToString()));
        }

        if (department.ParentId != null && await _context.Departments.AnyAsync(d => d.ParentId == department.ParentId && d.Slug == department.Slug, cancellationToken))
        {
            errors.Add(Errors.DepartmentErrors.SlugConflict(department.ParentId.Value, department.Slug.ToString()));
        }

        if (errors.Count > 0)
        {
            _logger.LogError("Failed to create department with id: {Id}", department.Id);

            return new Failure(errors);
        }

        await _context.Departments.AddAsync(department, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<Result<Department, Failure>> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == name, cancellationToken);

        if (department == null)
            return Errors.DepartmentErrors.NotFoudName().ToFailure();

        return department;
    }

    public async Task<Result<Department, Failure>> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department == null)
            return Errors.DepartmentErrors.NotFoud().ToFailure();

        return department;
    }

    public async Task<UnitResult<Failure>> AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocationsResults = locations.Select(l => DepartmentLocation.Create(department.Id, l.Id));

        if (departmentLocationsResults.Any(dl => dl.IsFailure))
            return new Failure(departmentLocationsResults.Where(dl => dl.IsFailure).SelectMany(dl => dl.Error));

        var departmentLocations = departmentLocationsResults.Select(dl => dl.Value);

        if(await _context.DepartmentLocations.AnyAsync(dl => 
            dl.DepartmentId == department.Id && locations.Select(l => l.Id).Contains(dl.LocationId), cancellationToken))
        {
            return Errors.DepartmentErrors.LocationConflict().ToFailure();
        }

        await _context.DepartmentLocations.AddRangeAsync(departmentLocations, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<UnitResult<Failure>> RemoveLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocations = await _context.DepartmentLocations.Where(dl => dl.DepartmentId == department.Id &&
            locations.Select(l => l.Id).Contains(dl.LocationId))
            .ToListAsync(cancellationToken);

        if (!await _context.DepartmentLocations.AnyAsync(dl =>
           dl.DepartmentId == department.Id && locations.Select(l => l.Id).Contains(dl.LocationId), cancellationToken))
        {
            return Errors.DepartmentErrors.LocationNotFound().ToFailure();
        }

        _context.DepartmentLocations.RemoveRange(departmentLocations);

        return UnitResult.Success<Failure>();
    }

    public async Task<Result<IEnumerable<Department>, Failure>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Departments.ToListAsync(cancellationToken);
    }
}
