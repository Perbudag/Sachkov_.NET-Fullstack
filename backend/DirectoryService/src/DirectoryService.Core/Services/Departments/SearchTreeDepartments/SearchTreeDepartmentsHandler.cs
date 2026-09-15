using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Services.Departments.SearchDepartments;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Shared;
using Path = DirectoryService.Domain.ValueObjects.Path;

namespace DirectoryService.Core.Services.Departments.SearchTreeDepartments;

internal class SearchTreeDepartmentsHandler : IQueryHandler<SearchTreeDepartmentsResultDto[], SearchTreeDepartmentsQuery>
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IValidator<SearchTreeDepartmentsQuery> _validator;

    public SearchTreeDepartmentsHandler(IDbConnectionFactory connectionFactory, IValidator<SearchTreeDepartmentsQuery> validator)
    {
        _connectionFactory = connectionFactory;
        _validator = validator;
    }

    public async Task<Result<SearchTreeDepartmentsResultDto[], Failure>> HandleAsync(SearchTreeDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var validateResilt = await _validator.ValidateAsync(query, cancellationToken);
        if (!validateResilt.IsValid)
            return validateResilt.ToErrors();

        using var connection = await _connectionFactory.CreateAsync(cancellationToken);
        var builder = new SqlBuilder();

        var template = builder.AddTemplate("""
            WITH roots AS(
                SELECT department_id, path 
                FROM departments 
                /**where**/
            )
            SELECT DISTINCT
                d.department_id As Id,
                d.name As Name,
                d.slug As Slug,
                d.path As Path,
                d.depth As Depth,
                EXISTS(SELECT *
                    FROM departments
                    WHERE path <@ d.path
                    AND department_id <> d.department_id) As HasChildren
            FROM departments d, roots r
            WHERE d.path @> r.path
                AND is_deleted = FALSE
            ORDER BY d.Depth;
            """);

        builder.Where("name LIKE '%' || @Q || '%'", new { query.Q });
        builder.Where("is_deleted = FALSE");

        var nodes = await connection.QueryAsync<DepartmentTreeNodeDto>(
            template.RawSql,
            template.Parameters);

        var roots = nodes.Where(n => n.Name.Contains(query.Q, StringComparison.CurrentCulture));

        var result = new List<SearchTreeDepartmentsResultDto>();

        foreach (var root in roots)
        {
            var ancestors = nodes.Where(n => !string.Equals(root.Path, n.Path, StringComparison.Ordinal) 
                                        && root.Path.StartsWith(n.Path + ".", StringComparison.Ordinal)).ToList();

            result.Add(new SearchTreeDepartmentsResultDto(root, ancestors));
        }

        return result.ToArray();
    }
}
