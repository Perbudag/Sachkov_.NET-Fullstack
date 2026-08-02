using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAll;

internal class GetAllDepartmentsHandler : IQueryHandler<DepartmentResponse[], GetAllDepartmentsQuery>
{
    private readonly IReadDbContext _repository;

    public GetAllDepartmentsHandler(IReadDbContext repository)
    {
        _repository = repository;
    }

    public async Task<Result<DepartmentResponse[], Failure>> HandleAsync(GetAllDepartmentsQuery query, CancellationToken cancellationToken)
    {
        var responses = await _repository.DepartmentsRead
            .Select(d => new DepartmentResponse(
                Id: d.Id,
                Name: d.Name.ToString(),
                Slug: d.Slug.ToString(),
                Path: d.Path.ToString(),
                ParentId: d.ParentId
            ))
            .ToArrayAsync(cancellationToken);

        return responses;
    }
}
