using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Domain.Entities;
using Shared;

namespace DirectoryService.Core.Services.Departments.GetAll;

internal class GetAllDepartmentHandler : IQueryHandler<DepartmentResponse[], GetAllDepartmentQuery>
{
    private readonly IDepartmentsRepository _repository;

    public GetAllDepartmentHandler(IDepartmentsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<DepartmentResponse[], Failure>> HandleAsync(GetAllDepartmentQuery query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAsync(cancellationToken);

        if (result.IsFailure)
            return result.Error;

        return result.Value.Select(d => new DepartmentResponse(
            d.Id, d.Name.ToString(), d.Slug.ToString(), d.Path.ToString(), d.ParentId)).ToArray();
    }
}
