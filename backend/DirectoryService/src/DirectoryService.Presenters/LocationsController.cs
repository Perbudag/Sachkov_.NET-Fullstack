using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        LocationResponse[] response = [new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            )];

        return Ok(response);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var response = new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            );

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
    {
        var response = new LocationResponse(
            Guid.CreateVersion7(),
            request.Name,
            request.Addess
            );

        return Ok(response);
    }


    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateLocationRequest request)
    {
        var response = new LocationResponse(
            request.Id,
            request.Name,
            request.Addess
            );

        return Ok(response);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        return Ok("delete request");
    }
}
