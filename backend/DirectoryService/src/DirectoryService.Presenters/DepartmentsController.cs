using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
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
        [FromServices] IQueryHandler<DepartmentResponse[], GetAllDepartmentQuery> handler,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync(new(), cancellationToken);
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
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync(request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<DepartmentResponse>> Update(
        [FromServices] ICommandHandler<DepartmentResponse, UpdateDepartmentCommand> handler,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync((id, request), cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return UnitResult.Success<Failure>();
    }


    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> AddLocation(
        [FromServices] ICommandHandler<AddLocationInDepartmentCommand> handler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync((departmentId, locationId), cancellationToken);
    }


    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> RemoveLocation(
        [FromServices] ICommandHandler<RemoveLocationInDepartmentCommand> handler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        return await handler.HandleAsync((departmentId, locationId), cancellationToken);
    }
}
