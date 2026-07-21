using Microsoft.AspNetCore.Http;
using Shared;

namespace DirectoryService.Presenters.Results;

public class FailureResult : IResult
{
    private readonly Failure _errors;

    public FailureResult(Error error)
    {
        _errors = error.ToFailure();
    }

    public FailureResult(Failure errors)
    {
        _errors = errors;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Failure(_errors);

        if(!_errors.Any())
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            return httpContext.Response.WriteAsJsonAsync(envelope);
        }

        var distinctErrorTypes = _errors
            .Select(x => x.Type)
            .Distinct()
            .ToList();

        httpContext.Response.StatusCode = distinctErrorTypes.Count > 1 ?
                StatusCodes.Status500InternalServerError :
                GetStatusCodeFromErrorType(distinctErrorTypes[0]);

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
}
