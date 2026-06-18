using DirectoryService.Core.Departments;
using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;

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
        var departmentId = await departmentsService.CreateAsync(request, cancellationToken);

        return Ok(departmentId);
    }


    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await departmentsService.UpdateAsync(id, request, cancellationToken);

        return Ok(response);
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
        try
        {
            await departmentsService.AddLocationAsync(departmentId, locationId, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RemoveLocation(
        [FromServices] IDepartmentsService departmentsService,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await departmentsService.RemoveLocationAsync(departmentId, locationId, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
