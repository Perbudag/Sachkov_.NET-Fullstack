using CSharpFunctionalExtensions;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System.Linq.Expressions;

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

        if (await _context.Departments
            .IgnoreQueryFilters()
            .AnyAsync(d => d.Name == department.Name, cancellationToken))
        {
            errors.Add(Errors.DepartmentErrors.Conflict(department.Name.ToString()));
        }

        if (department.ParentId != null && await _context.Departments
            .IgnoreQueryFilters()
            .AnyAsync(d => d.ParentId == department.ParentId && d.Slug == department.Slug, cancellationToken))
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

    public async Task<Result<Department, Failure>> GetByAsync(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken)
    {
        var query = _context.Departments.AsQueryable();

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        var department = await query.FirstOrDefaultAsync(predicate, cancellationToken);

        if (department == null)
            return Errors.DepartmentErrors.NotFoud().ToFailure();

        return department;
    }

    public Task<Result<Department, Failure>> GetByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken) =>
        GetByAsync(predicate, false, cancellationToken);

    public IAsyncEnumerable<Department> GetByAsyncEnum(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters = false)
    {
        var query = _context.Departments.AsQueryable();

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.Where(predicate).AsAsyncEnumerable();
    }

    public async Task<UnitResult<Failure>> AddLocationsAsync(Department department, IEnumerable<Location> locations, CancellationToken cancellationToken)
    {
        var departmentLocationsResults = locations.Select(l => DepartmentLocation.Create(department.Id, l.Id));

        if (departmentLocationsResults.Any(dl => dl.IsFailure))
            return new Failure(departmentLocationsResults.Where(dl => dl.IsFailure).SelectMany(dl => dl.Error));

        var departmentLocations = departmentLocationsResults.Select(dl => dl.Value);

        if (await _context.DepartmentLocations.AnyAsync(dl =>
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

    public async Task<UnitResult<Failure>> AddPositionsAsync(Department department, IEnumerable<Position> positions, CancellationToken cancellationToken)
    {
        var departmentPositionsResults = positions.Select(p => DepartmentPosition.Create(department.Id, p.Id));

        if (departmentPositionsResults.Any(dl => dl.IsFailure))
            return new Failure(departmentPositionsResults.Where(dl => dl.IsFailure).SelectMany(dl => dl.Error));

        var departmentPositions = departmentPositionsResults.Select(dl => dl.Value);

        if (await _context.DepartmentsPositions.AnyAsync(dp =>
            dp.DepartmentId == department.Id && positions.Select(p => p.Id).Contains(dp.PositionId), cancellationToken))
        {
            return Errors.DepartmentErrors.PositionConflict().ToFailure();
        }

        await _context.DepartmentsPositions.AddRangeAsync(departmentPositions, cancellationToken);

        return UnitResult.Success<Failure>();
    }

    public async Task<UnitResult<Failure>> RemovePositionsAsync(Department department, IEnumerable<Position> positions, CancellationToken cancellationToken)
    {
        var departmentPositions = await _context.DepartmentsPositions.Where(dp => dp.DepartmentId == department.Id &&
            positions.Select(p => p.Id).Contains(dp.PositionId))
            .ToListAsync(cancellationToken);

        if (!await _context.DepartmentsPositions.AnyAsync(dp =>
           dp.DepartmentId == department.Id && positions.Select(p => p.Id).Contains(dp.PositionId), cancellationToken))
        {
            return Errors.DepartmentErrors.PositionNotFound().ToFailure();
        }

        _context.DepartmentsPositions.RemoveRange(departmentPositions);

        return UnitResult.Success<Failure>();
    }

    public Task<long> CountByAsync(Expression<Func<Department, bool>> predicate, bool ignoreQueryFilters, CancellationToken cancellationToken)
    {
        var query = _context.Departments.AsQueryable();

        if(ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return query.LongCountAsync(predicate, cancellationToken);
    }

    public Task<long> CountByAsync(Expression<Func<Department, bool>> predicate, CancellationToken cancellationToken) =>
          CountByAsync(predicate, false, cancellationToken);
}
