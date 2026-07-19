using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Presenters.Extensions;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        DepartmentResponse[] response = [new DepartmentResponse(
            Guid.CreateVersion7(),
            "TestName",
            "TestSlug",
            "TestParentSlug.TestSlug"
            )];

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var response = new DepartmentResponse(
            Guid.CreateVersion7(),
            "TestName",
            "TestSlug",
            "TestParentSlug.TestSlug"
            );

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] IDepartmentsService departmentsService,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await departmentsService.CreateAsync(request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result);
    }


    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await departmentsService.UpdateAsync(id, request, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok(result);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }


    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> AddLocation(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        var result = await departmentsService.AddLocationAsync(departmentId, locationId, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }


    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RemoveLocation(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        var result = await departmentsService.RemoveLocationAsync(departmentId, locationId, cancellationToken);

        if (result.IsFailure)
            return result.Error.ToResponse();

        return Ok();
    }
}
