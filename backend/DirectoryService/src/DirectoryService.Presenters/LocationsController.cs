using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Presenters.Results;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<LocationResponse[]>> GetAll(CancellationToken cancellationToken = default)
    {
        return Result.Success<LocationResponse[], Failure>([new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            )]);
    }


    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<LocationResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Result.Success<LocationResponse, Failure>(new LocationResponse(
            Guid.CreateVersion7(),
            "TestName",
            new AddressDto("352941", "Россия", "Кемеровская область", "Армавир", "Зеленый пер.", "103", "24")
            ));
    }


    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ILocationsService locationsService,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await locationsService.CreateAsync(request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<LocationResponse>> Update(
        [FromServices] ILocationsService locationsService,
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await locationsService.UpdateAsync(id, request, cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Result.Success<Failure>();
    }
}
