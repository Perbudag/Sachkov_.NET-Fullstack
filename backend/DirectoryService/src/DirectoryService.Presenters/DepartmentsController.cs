using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Core.Services.Departments.AddLocation;
using DirectoryService.Core.Services.Departments.Create;
using DirectoryService.Core.Services.Departments.GetAll;
using DirectoryService.Core.Services.Departments.RemoveLocation;
using DirectoryService.Core.Services.Departments.Update;
using DirectoryService.Presenters.Results;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<DepartmentResponse[]>> GetAll(
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync(new GetAllDepartmentQuery(), cancellationToken);
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
        [FromServices] ISender sender,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((CreateDepartmentCommand)request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<DepartmentResponse>> Update(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((UpdateDepartmentCommand)(id, request), cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return UnitResult.Success<Failure>();
    }


    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> AddLocation(
        [FromServices] ISender sender,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((AddLocationInDepartmentCommand)(departmentId, locationId), cancellationToken);
    }


    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> RemoveLocation(
        [FromServices] ISender sender,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((RemoveLocationInDepartmentCommand)(departmentId, locationId), cancellationToken);
    }
}
