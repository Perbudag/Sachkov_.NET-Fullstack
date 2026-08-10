using DirectoryService.Contracts.Departments;
using DirectoryService.Core;
using DirectoryService.Core.Services.Departments.AddLocation;
using DirectoryService.Core.Services.Departments.AddPosition;
using DirectoryService.Core.Services.Departments.Create;
using DirectoryService.Core.Services.Departments.Delete;
using DirectoryService.Core.Services.Departments.GetAll;
using DirectoryService.Core.Services.Departments.GetById;
using DirectoryService.Core.Services.Departments.RemoveLocation;
using DirectoryService.Core.Services.Departments.RemovePosition;
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
    public async Task<EndpointResult<PageResult<DepartmentListItemDto[]>>> GetAll(
        [FromServices] ISender sender,
        [FromQuery] GetAllDepartmentsRequest requst,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((GetAllDepartmentsQuery)requst, cancellationToken);
    }


    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<DepartmentDto>> GetById(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((GetByIdDepartmentQuery)id, cancellationToken);
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
    public async Task<EndpointResult<DepartmentDto>> Update(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((UpdateDepartmentCommand)(id, request), cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((DeleteDepartmentCommand)id, cancellationToken);
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


    [HttpPost("/departments/{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult> AddPosition(
        [FromServices] ISender sender,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((AddPositionDepartmentCommand)(departmentId, positionId), cancellationToken);
    }


    [HttpDelete("/departments/{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult> RemovePosition(
        [FromServices] ISender sender,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((RemovePositionDepartmentCommand)(departmentId, positionId), cancellationToken);
    }
}
