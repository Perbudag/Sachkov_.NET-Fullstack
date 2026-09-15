using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using DirectoryService.Core.Validation;
using FluentValidation;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAncestors
{
    internal class GetDepartmentAncestorsHandler : IQueryHandler<PageResult<DepartmentTreeNodeDto[]>, GetDepartmentAncestorsQuery>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IValidator<GetDepartmentAncestorsQuery> _validator;

        public GetDepartmentAncestorsHandler(IDbConnectionFactory connectionFactory, IValidator<GetDepartmentAncestorsQuery> validator)
        {
            _connectionFactory = connectionFactory;
            _validator = validator;
        }

        public async Task<Result<PageResult<DepartmentTreeNodeDto[]>, Failure>> HandleAsync(GetDepartmentAncestorsQuery query, CancellationToken cancellationToken)
        {
            var validateResilt = await _validator.ValidateAsync(query, cancellationToken);
            if (!validateResilt.IsValid)
                return validateResilt.ToErrors();

            using var connection = await _connectionFactory.CreateAsync(cancellationToken);
            var builder = new SqlBuilder();
            long totalCount = 0;

            var template = builder.AddTemplate("""
                WITH root AS(
                    SELECT department_id, path 
                    FROM departments 
                    /**where**/
                )
                SELECT 
                    d.department_id As Id,
                    d.name As Name,
                    d.slug As Slug,
                    d.path As Path,
                    d.depth As Depth,
                    EXISTS(SELECT *
                        FROM departments
                        WHERE path <@ d.path
                        AND department_id <> d.department_id) As HasChildren,
                    COUNT(*) OVER() As TotalCount
                FROM departments d, root r
                WHERE d.path @> r.path
                AND d.department_id <> r.department_id
                /**orderby**/
                OFFSET @Offset LIMIT @Limit;
                """);

            builder.AddParameters(new
            {
                Offset = (query.Page - 1) * query.PageSize,
                Limit = query.PageSize
            });

            builder.Where("department_id = @ParentId", new { query.ParentId });
            builder.Where("is_deleted = FALSE");

            string? SordBy = query.SortBy switch
            {
                string s when s.Equals(nameof(DepartmentTreeNodeDto.Id), StringComparison.OrdinalIgnoreCase) => "d.department_id",
                string s when s.Equals(nameof(DepartmentTreeNodeDto.Name), StringComparison.OrdinalIgnoreCase) => "d.name",
                string s when s.Equals(nameof(DepartmentTreeNodeDto.Slug), StringComparison.OrdinalIgnoreCase) => "d.slug",
                string s when s.Equals(nameof(DepartmentTreeNodeDto.Path), StringComparison.OrdinalIgnoreCase) => "d.path",
                string s when s.Equals(nameof(DepartmentTreeNodeDto.Depth), StringComparison.OrdinalIgnoreCase) => "d.depth",
                _ => null
            };

            if (SordBy == null)
            {
                return Errors.DepartmentErrors.ValidationError("Некорректное поле сортировки", nameof(query.SortBy)).ToFailure();
            }

            if (string.Equals(query.SortDir, "asc", StringComparison.OrdinalIgnoreCase))
            {
                builder.OrderBy($"{SordBy} ASC");
            }
            else if (string.Equals(query.SortDir, "desc", StringComparison.OrdinalIgnoreCase))
            {
                builder.OrderBy($"{SordBy} DESC");
            }
            else
            {
                return Errors.DepartmentErrors.ValidationError("Значениями SortDir могут быть только \"asc\" и \"desc\"",
                    nameof(query.SortDir)).ToFailure();
            }

            var result = await connection.QueryAsync<DepartmentTreeNodeDto, long, DepartmentTreeNodeDto>(
            template.RawSql,
            (departmentNode, total) =>
            {
                totalCount = total;
                return departmentNode;
            },
            template.Parameters,
            splitOn: "TotalCount");

            if (query.Page > 1 && totalCount == 0)
                return Errors.DepartmentErrors.ValidationError($"Номер страницы превысил максимальное значение",
                    nameof(query.Page)).ToFailure();

            return new PageResult<DepartmentTreeNodeDto[]>(result.ToArray(), totalCount, query.Page, query.PageSize);
        }
    }
}
