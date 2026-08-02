using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Core;
using DirectoryService.Core.Services.Locations.Create;
using DirectoryService.Core.Services.Locations.Delete;
using DirectoryService.Core.Services.Locations.GetAll;
using DirectoryService.Core.Services.Locations.GetById;
using DirectoryService.Core.Services.Locations.Update;
using DirectoryService.Presenters.Results;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<LocationResponse[]>> GetAll(
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync(new GetAllLocationsQuery(), cancellationToken);
    }


    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<LocationResponse>> GetById(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((GetByIdLocationQuery)id, cancellationToken);
    }


    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ISender sender,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((CreateLocationCommand)request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<LocationResponse>> Update(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((UpdateLocationCommand)(id, request), cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((DeleteLocationCommand)id, cancellationToken);
    }
}
