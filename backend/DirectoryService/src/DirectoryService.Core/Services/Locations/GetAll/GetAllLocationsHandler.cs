using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;

namespace DirectoryService.Core.Services.Locations.GetAll;


internal class GetAllLocationsHandler : IQueryHandler<PageResult<LocationListItemDto[]>, GetAllLocationsQuery>
{
    private readonly IReadDbContext _context;
    private readonly IValidator<GetAllLocationsQuery> _validator;

    public GetAllLocationsHandler(IReadDbContext context, IValidator<GetAllLocationsQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<PageResult<LocationListItemDto[]>, Failure>> HandleAsync(GetAllLocationsQuery query, CancellationToken cancellationToken)
    {
        var validateResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validateResult.IsValid)
            return validateResult.ToErrors();


        var isAscending = string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
        var isDescending = string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase);

        if (!isAscending && !isDescending)
            return Errors.DepartmentErrors.ValidationError("Значениями SortOrder могут быть только \"asc\" и \"desc\"",
                nameof(query.SortDir)).ToFailure();

        var locationQuery = from l in _context.LocationsRead
                            join dl in _context.DepartmentLocationsRead
                                on l.Id equals dl.LocationId into joinedDepartments
                            select new
                            {
                                Location = l,
                                DepartmentsCount = joinedDepartments.Count()
                            };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            locationQuery = locationQuery.Where(x => EF.Functions.Like((string)(object)x.Location.Name, $"%{query.Search}%"));
        }

        if (query.MinDepartmentCount > 0)
        {
            locationQuery = locationQuery.Where(x => x.DepartmentsCount >= query.MinDepartmentCount);
        }

        var totalCount = await locationQuery.LongCountAsync(cancellationToken);

        var maxPageNumber = (long)Math.Ceiling((double)totalCount / query.PageSize);
        if (maxPageNumber == 0) maxPageNumber = 1;

        if (query.Page > maxPageNumber)
            return Errors.LocationErrors.ValidationError($"Номер страницы превысил максимальное значение (макс. {maxPageNumber})",
                nameof(query.Page)).ToFailure();

        locationQuery = query.SortBy switch
        {
            string s when s.Equals(nameof(LocationListItemDto.Id), StringComparison.OrdinalIgnoreCase) =>
                isAscending ? locationQuery.OrderBy(x => x.Location.Id) : locationQuery.OrderByDescending(x => x.Location.Id),

            string s when s.Equals(nameof(LocationListItemDto.Name), StringComparison.OrdinalIgnoreCase) =>
                isAscending ? locationQuery.OrderBy(x => x.Location.Name) : locationQuery.OrderByDescending(x => x.Location.Name),

            string s when s.Equals(nameof(LocationListItemDto.Address), StringComparison.OrdinalIgnoreCase) =>
                isAscending ? locationQuery.OrderBy(x => x.Location.Address) : locationQuery.OrderByDescending(x => x.Location.Address),

            string s when s.Equals(nameof(LocationListItemDto.CreatedAt), StringComparison.OrdinalIgnoreCase) =>
                isAscending ? locationQuery.OrderBy(x => x.Location.CreatedAt) : locationQuery.OrderByDescending(x => x.Location.CreatedAt),

            string s when s.Equals(nameof(LocationListItemDto.DepartmentCount), StringComparison.OrdinalIgnoreCase) =>
                isAscending ? locationQuery.OrderBy(x => x.DepartmentsCount) : locationQuery.OrderByDescending(x => x.DepartmentsCount),

            _ => null
        };

        if (locationQuery == null)
            return Errors.LocationErrors.ValidationError("Некорректное поле сортировки", nameof(query.SortBy)).ToFailure();

        var responses = await locationQuery
        .Skip((query.Page - 1) * query.PageSize)
        .Take(query.PageSize)
        .Select(x => new LocationListItemDto(
            id: x.Location.Id,
            name: x.Location.Name.ToString(),
            address: new AddressDto(
                PostalCode: x.Location.Address.PostalCode,
                Country: x.Location.Address.Country,
                Region: x.Location.Address.Region,
                City: x.Location.Address.City,
                Street: x.Location.Address.Street,
                House: x.Location.Address.House,
                Apartment: x.Location.Address.Apartment),
            createdAt: x.Location.CreatedAt,
            departmentCount: x.DepartmentsCount
        ))
        .ToArrayAsync(cancellationToken);

        return new PageResult<LocationListItemDto[]>(responses, totalCount, query.Page, query.PageSize);
    }
}
