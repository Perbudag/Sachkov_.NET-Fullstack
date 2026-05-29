using DirectoryService.Contracts.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
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
    public async Task<IActionResult> GetById([FromRoute] Guid id)
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
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        var response = new DepartmentResponse(
            Guid.CreateVersion7(),
            request.Name,
            request.Slug,
            "TestParentSlug." + request.Slug,
            request.ParentId
            );

        return Ok(response);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var response = new DepartmentResponse(
            id,
            request.Name,
            request.Slug,
            "TestParentSlug." + request.Slug,
            request.ParentId
            );

        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return Ok();
    }
}
