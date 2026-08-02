using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
using DirectoryService.Core;
using DirectoryService.Core.Services.Positions.Create;
using DirectoryService.Core.Services.Positions.Delete;
using DirectoryService.Core.Services.Positions.Update;
using DirectoryService.Presenters.Results;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DirectoryService.Presenters;


[Route("api/[controller]")]
[ApiController]
public class PositionsController : ControllerBase
{
    [HttpGet]
    public async Task<EndpointResult<PositionResponse[]>> GetAll(CancellationToken cancellationToken = default)
    {
        return Result.Success<PositionResponse[], Failure>([ new PositionResponse(
            Guid.CreateVersion7(),
            "testName"
        )]);
    }


    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<PositionResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Result.Success<PositionResponse, Failure>(new PositionResponse(
            id,
            "testName"
        ));
    }


    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ISender sender,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((CreatePositionCommand)request, cancellationToken);
    }


    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<PositionResponse>> Update(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        [FromBody] UpdatePositionRequest request,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((UpdatePositionCommand)(id, request), cancellationToken);
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete(
        [FromServices] ISender sender,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        return await sender.SendAsync((DeletePositionCommand)id, cancellationToken);
    }
}