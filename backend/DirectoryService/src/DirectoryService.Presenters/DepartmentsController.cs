using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Presenters.Results;
using Shared;
using CSharpFunctionalExtensions;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<DepartmentResponse[]>> GetAll(CancellationToken cancellationToken = default)
    {
        return Result.Success<DepartmentResponse[], Failure>([new DepartmentResponse(
            Guid.CreateVersion7(),
            "TestName",
            "TestSlug",
            "TestParentSlug.TestSlug"
            )]);
    }


    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<DepartmentResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Result.Success<DepartmentResponse, Failure>(new DepartmentResponse(
                Guid.CreateVersion7(),
                "TestName",
                "TestSlug",
                "TestParentSlug.TestSlug"
            ));
    }


    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] IDepartmentsService departmentsService,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await departmentsService.CreateAsync(request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<DepartmentResponse>> Update(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await departmentsService.UpdateAsync(id, request, cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return UnitResult.Success<Failure>();
    }


    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> AddLocation(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await departmentsService.AddLocationAsync(departmentId, locationId, cancellationToken);
    }


    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> RemoveLocation(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await departmentsService.RemoveLocationAsync(departmentId, locationId, cancellationToken);
    }
}
