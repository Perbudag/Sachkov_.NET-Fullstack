using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.Framework.Endpoints;
using Shared.Kernel;


namespace Shared.Framework.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(Envelope.Failure(Error.Failure("something went wrong")), cancellationToken: context.RequestAborted);
        }
    }
}
