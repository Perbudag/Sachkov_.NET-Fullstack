using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }

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
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _locationsService.CreateAsync(request, cancellationToken);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken = default)
    {
        var response = new LocationResponse(
            id,
            request.Name,
            request.Address
            );

        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Ok();
    }
}
