using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Departments.GetChildren;

internal class GetDepartmentChildrenHandler : IQueryHandler<PageResult<DepartmentTreeNodeDto[]>, GetDepartmentChildrenQuery>
{
    private readonly IReadDbContext _dbContext;
    private readonly IValidator<GetDepartmentChildrenQuery> _validator;

    public GetDepartmentChildrenHandler(IReadDbContext dbContext, IValidator<GetDepartmentChildrenQuery> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<Result<PageResult<DepartmentTreeNodeDto[]>, Failure>> HandleAsync(GetDepartmentChildrenQuery query, CancellationToken cancellationToken)
    {
        var validateResilt = await _validator.ValidateAsync(query, cancellationToken);
        if (!validateResilt.IsValid)
            return validateResilt.ToErrors();

        var departmentsQuery = _dbContext.DepartmentsRead.Where(d => d.ParentId == query.ParentId);

        var totalCount = await departmentsQuery.CountAsync(cancellationToken);
        var maxPageNumber = totalCount / query.PageSize;

        if ((totalCount % query.PageSize) > 0)
            maxPageNumber++;

        if (query.Page > maxPageNumber && maxPageNumber != 0)
            return Errors.DepartmentErrors.ValidationError($"Номер страницы превысил максимальное значение (макс. {maxPageNumber})",
                nameof(query.Page)).ToFailure();

        var isAscending = string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
        var isDescending = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        if (!isAscending && !isDescending)
            return Errors.DepartmentErrors.ValidationError("Значениями SortOrder могут быть только \"asc\" и \"desc\"",
                nameof(query.SortDir)).ToFailure();

        Expression<Func<Department, object>>? keySelector = query.SortBy switch
        {
            string s when s.Equals(nameof(DepartmentTreeNodeDto.Id), StringComparison.OrdinalIgnoreCase) => d => d.Id,
            string s when s.Equals(nameof(DepartmentTreeNodeDto.Name), StringComparison.OrdinalIgnoreCase) => d => d.Name,
            string s when s.Equals(nameof(DepartmentTreeNodeDto.Slug), StringComparison.OrdinalIgnoreCase) => d => d.Slug,
            string s when s.Equals(nameof(DepartmentTreeNodeDto.Path), StringComparison.OrdinalIgnoreCase) => d => d.Path,
            string s when s.Equals(nameof(DepartmentTreeNodeDto.Depth), StringComparison.OrdinalIgnoreCase) => d => d.Depth,
            _ => null
        };

#pragma warning disable CA1508 // Предотвращение появления неиспользуемого условного кода
        if (keySelector == null)
            return Errors.DepartmentErrors.ValidationError("Некорректное поле сортировки", nameof(query.SortBy)).ToFailure();
#pragma warning restore CA1508 // Предотвращение появления неиспользуемого условного кода

        departmentsQuery = isAscending
            ? departmentsQuery.OrderBy(keySelector)
            : departmentsQuery.OrderByDescending(keySelector);

        var response = await departmentsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DepartmentTreeNodeDto(
                Id: d.Id,
                Name: d.Name.ToString(),
                Slug: d.Slug.ToString(),
                Path: d.Path.ToString(),
                Depth: d.Depth,
                HasChildren: _dbContext.DepartmentsRead.Any(c => c.ParentId == d.Id)))
            .ToArrayAsync(cancellationToken);

        return new PageResult<DepartmentTreeNodeDto[]>(response, totalCount, query.Page, query.PageSize);
    }
}
