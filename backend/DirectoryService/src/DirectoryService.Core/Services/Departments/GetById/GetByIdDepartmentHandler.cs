using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Fails;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetById;

internal class GetByIdDepartmentHandler : IQueryHandler<DepartmentResponse, GetByIdDepartmentQuery>
{
    private readonly IReadDbContext _context;

    public GetByIdDepartmentHandler(IReadDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DepartmentResponse, Failure>> HandleAsync(GetByIdDepartmentQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
            return Errors.SharedErrors.IsRequired("Id", "departments.validation.error").ToFailure();


        var respomse = await _context.DepartmentsRead
            .Where(d => d.Id == query.Id)
            .Select(d => new DepartmentResponse(
                Id: d.Id,
                Name: d.Name.ToString(),
                Slug: d.Slug.ToString(),
                Path: d.Path.ToString(),
                ParentId: d.ParentId
            ))
            .FirstOrDefaultAsync(cancellationToken);


        if (respomse == null)
            return Errors.DepartmentErrors.NotFoud().ToFailure();


        return respomse;
    }
}
