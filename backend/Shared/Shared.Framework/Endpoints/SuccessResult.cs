using Microsoft.AspNetCore.Http;

namespace Shared.Framework.Endpoints;

public class SuccessResult<TValue> : IResult
{
    private readonly TValue _value;

    public SuccessResult(TValue value)
    {
        _value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {

        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok(_value);

        httpContext.Response.StatusCode = StatusCodes.Status200OK;

        return httpContext.Response.WriteAsJsonAsync(envelope, cancellationToken: httpContext.RequestAborted);
    }
}
