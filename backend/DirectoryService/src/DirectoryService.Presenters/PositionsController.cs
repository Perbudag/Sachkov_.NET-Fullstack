using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Positions;
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
    public async Task<EndpointResult<PositionResponse>> Create([FromBody] CreatePositionRequest request, CancellationToken cancellationToken = default)
    {
        return Result.Success<PositionResponse, Failure>(new PositionResponse(
            Guid.CreateVersion7(),
            request.Name
        ));
    }


    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<PositionResponse>> Update([FromRoute] Guid id, [FromBody] UpdatePositionRequest request, CancellationToken cancellationToken = default)
    {
        return Result.Success<PositionResponse, Failure>(new PositionResponse(
            id,
            request.Name
        ));
    }


    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        return Result.Success<Failure>();
    }
}