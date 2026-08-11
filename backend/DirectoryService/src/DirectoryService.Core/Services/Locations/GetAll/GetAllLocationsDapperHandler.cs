using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Services.Locations.GetAll;

internal class GetAllLocationsDapperHandler : IQueryHandler<PageResult<LocationListItemDto[]>, GetAllLocationsDapperQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<GetAllLocationsDapperQuery> _validator;

    public GetAllLocationsDapperHandler(IDbConnectionFactory connectionFactory, IValidator<GetAllLocationsDapperQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<Result<PageResult<LocationListItemDto[]>, Failure>> HandleAsync(GetAllLocationsDapperQuery query, CancellationToken cancellationToken)
    {
        var validateResult = await _validator.ValidateAsync(query, cancellationToken);
        if (!validateResult.IsValid)
            return validateResult.ToErrors();

        using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        var builder = new SqlBuilder();
        long totalCount = 0;


        var template = builder.AddTemplate("""
            WITH location_pages AS (
                SELECT 
                    l.location_id, 
                    l.name, 
                    l.address, 
                    l.created_at,
                    count(dl.location_id)::int AS departments_count
                FROM locations AS l
                LEFT JOIN departments_locations AS dl ON l.location_id = dl.location_id
                /**where**/
                GROUP BY l.location_id, l.address, l.name
                /**having**/)
            SELECT 
                lp.location_id As Id,
                lp.name As Name,
                lp.address As Address,
                lp.created_at As CreatedAt,
                lp.departments_count As DepartmentCount,
                count(*) OVER() AS TotalCount
            FROM location_pages AS lp
            /**orderby**/
            OFFSET @Offset LIMIT @Limit;
            """);

        builder.AddParameters(new
        {
            Offset = (query.Page - 1) * query.PageSize,
            Limit = query.PageSize
        });

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            builder.Where("name LIKE '%' || @Search || '%'", new { query.Search });
        }

        if (query.MinDepartmentCount > 0)
        {
            builder.Having("count(dl.location_id)::int >= @MinDepartmentCount", new { query.MinDepartmentCount });
        }

        string? SordBy = query.SortBy switch
        {
            string s when s.Equals(nameof(LocationListItemDto.Id), StringComparison.OrdinalIgnoreCase) => "location_id",
            string s when s.Equals(nameof(LocationListItemDto.Name), StringComparison.OrdinalIgnoreCase) => "name",
            string s when s.Equals(nameof(LocationListItemDto.Address), StringComparison.OrdinalIgnoreCase) => "address",
            string s when s.Equals(nameof(LocationListItemDto.CreatedAt), StringComparison.OrdinalIgnoreCase) => "created_at",
            string s when s.Equals(nameof(LocationListItemDto.DepartmentCount), StringComparison.OrdinalIgnoreCase) => "departments_count",
            _ => null
        };

        if (SordBy == null)
        {
            return Errors.LocationErrors.ValidationError("Некорректное поле сортировки", nameof(query.SortBy)).ToFailure();
        }

        if (string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase))
        {
            builder.OrderBy($"{SordBy} ASC");
        }
        else if(string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase))
        {
            builder.OrderBy($"{SordBy} DESC");
        }
        else
        {
            return Errors.LocationErrors.ValidationError("Значениями SortOrder могут быть только \"asc\" и \"desc\"",
                nameof(query.SortDir)).ToFailure();
        }

        var result = await connection.QueryAsync<LocationListItemDto, long, LocationListItemDto>(
            template.RawSql,
            (location, total) =>
            {
                totalCount = total;
                return location;
            },
            template.Parameters,
            splitOn: "TotalCount");

        if (query.Page > 1 && totalCount == 0)
            return Errors.LocationErrors.ValidationError($"Номер страницы превысил максимальное значение",
                nameof(query.Page)).ToFailure();

        return new PageResult<LocationListItemDto[]>(result.ToArray(), totalCount, query.Page, query.PageSize);
    }
}
