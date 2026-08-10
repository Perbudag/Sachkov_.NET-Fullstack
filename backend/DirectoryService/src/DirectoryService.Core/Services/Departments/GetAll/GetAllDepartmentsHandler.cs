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
using System.Globalization;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Departments.GetAll;

internal class GetAllDepartmentsHandler : IQueryHandler<PageResult<DepartmentListItemDto[]>, GetAllDepartmentsQuery>
{
    private readonly IReadDbContext _repository;
    private readonly IValidator<GetAllDepartmentsQuery> _validator;

    public GetAllDepartmentsHandler(IReadDbContext repository, IValidator<GetAllDepartmentsQuery> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<Result<PageResult<DepartmentListItemDto[]>, Failure>> HandleAsync(GetAllDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var validateResilt = await _validator.ValidateAsync(query, cancellationToken);
        if (!validateResilt.IsValid)
            return validateResilt.ToErrors();

        var departmentQuery = _repository.DepartmentsRead;

        if (query.Search != null)
        {
            departmentQuery = departmentQuery.Where(d => EF.Functions.Like((string)(object)d.Name, $"%{query.Search}%"));
        }

        var totalCount = await departmentQuery.LongCountAsync(cancellationToken);
        var maxPageNumber = totalCount / query.PageSize;

        if ((totalCount % query.PageSize) > 0)
            maxPageNumber++;

        if (query.Page > maxPageNumber)
            return Errors.DepartmentErrors.ValidationError($"Номер страницы превысил максимальное значение (макс. {maxPageNumber})",
                nameof(query.Page)).ToFailure();


        var isAscending = string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
        var isDescending = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        if (!isAscending && !isDescending)
            return Errors.DepartmentErrors.ValidationError("Значениями SortOrder могут быть только \"asc\" и \"desc\"",
                nameof(query.SortDir)).ToFailure();

        Expression<Func<Department, object>>? keySelector = query.SortBy switch
        {
            string s when s.Equals(nameof(DepartmentListItemDto.Id), StringComparison.OrdinalIgnoreCase) => d => d.Id,
            string s when s.Equals(nameof(DepartmentListItemDto.Name), StringComparison.OrdinalIgnoreCase) => d => d.Name,
            string s when s.Equals(nameof(DepartmentListItemDto.Slug), StringComparison.OrdinalIgnoreCase) => d => d.Slug,
            string s when s.Equals(nameof(DepartmentListItemDto.CreatedAt), StringComparison.OrdinalIgnoreCase) => d => d.CreatedAt,
            _ => null
        };

#pragma warning disable CA1508 // Предотвращение появления неиспользуемого условного кода
        if (keySelector == null)
            return Errors.DepartmentErrors.ValidationError("Некорректное поле сортировки", nameof(query.SortBy)).ToFailure();
#pragma warning restore CA1508 // Предотвращение появления неиспользуемого условного кода

        departmentQuery = isAscending
            ? departmentQuery.OrderBy(keySelector)
            : departmentQuery.OrderByDescending(keySelector);


        var responses = await departmentQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DepartmentListItemDto(
                Id: d.Id,
                Name: d.Name.ToString(),
                Slug: d.Slug.ToString(),
                CreatedAt: d.CreatedAt
            ))
            .ToArrayAsync(cancellationToken);

        return new PageResult<DepartmentListItemDto[]>(responses, totalCount, query.Page, query.PageSize);
    }
}
