using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Shared;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presenters.Results;

public class EndpointResult<TValue> : IResult
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

    public Task ExecuteAsync(HttpContext httpContext) =>
            _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Failure> result) => new(result);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result) => new(result);
}

public class EndpointResult : EndpointResult<bool>
{
    public EndpointResult(UnitResult<Failure> result) : base(result.IsSuccess ? true : result.Error) { }
    public EndpointResult(UnitResult<Error> result) : base(result.IsSuccess ? true : result.Error) { }


    public static implicit operator EndpointResult(UnitResult<Failure> result) => new(result);

    public static implicit operator EndpointResult(UnitResult<Error> result) => new(result);
}