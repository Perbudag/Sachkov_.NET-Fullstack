using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Services.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        LocationResponse[] response = [new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            )];

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var response = new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            );

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ILocationsService locationsService,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await locationsService.CreateAsync(request, cancellationToken);

        return Ok(response);
    }


    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] ILocationsService locationsService,
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await locationsService.UpdateAsync(id, request, cancellationToken);

        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
