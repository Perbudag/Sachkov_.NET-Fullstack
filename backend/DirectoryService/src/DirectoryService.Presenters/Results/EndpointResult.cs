using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Shared;
using System.Reflection;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presenters.Results;

public class EndpointResult<TValue> : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(Result<TValue, Failure> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new FailureResult(result.Error);

    }

    public EndpointResult(Result<TValue, Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new FailureResult(result.Error);

    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(200, typeof(Envelope<TValue>), ["application/json"]));

        builder.Metadata.Add(new ProducesResponseTypeMetadata(400, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(404, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(409, typeof(Envelope<TValue>), ["application/json"]));

        builder.Metadata.Add(new ProducesResponseTypeMetadata(500, typeof(Envelope<TValue>), ["application/json"]));
    }

    public Task ExecuteAsync(HttpContext httpContext) =>
            _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Failure> result) => new(result);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result) => new(result);
}

public class EndpointResult : EndpointResult<bool>
{
    public EndpointResult(UnitResult<Failure> result) :
        base(result.IsSuccess
        ? Result.Success<bool, Failure>(true)
        : Result.Failure<bool, Failure>(result.Error))
    { }

    public EndpointResult(UnitResult<Error> result) :
        base(result.IsSuccess
        ? Result.Success<bool, Failure>(true)
        : Result.Failure<bool, Failure>(result.Error))
    { }


    public static implicit operator EndpointResult(UnitResult<Failure> result) => new(result);

    public static implicit operator EndpointResult(UnitResult<Error> result) => new(result);
}