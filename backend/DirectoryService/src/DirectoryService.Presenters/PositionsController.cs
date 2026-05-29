using DirectoryService.Contracts.Positions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        PositionResponse[] response = [ new PositionResponse(
            Guid.CreateVersion7(),
            "testName"
        )];

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var response = new PositionResponse(
            id,
            "testName"
        );

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new PositionResponse(
            Guid.CreateVersion7(),
            request.Name
        );

        return Ok(response);
    }


    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePositionRequest request, CancellationToken cancellationToken = default)
    {
        var response = new PositionResponse(
            request.Id,
            request.Name
        );

        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}