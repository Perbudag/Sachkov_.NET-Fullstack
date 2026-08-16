using Shared;
using System.Text.Json;

namespace DirectoryService.Web.Middlewares;

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
#pragma warning disable CA2254 // Шаблон должен быть статическим выражением
            _logger.LogError(ex, ex.Message);
#pragma warning restore CA2254 // Шаблон должен быть статическим выражением

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(Envelope.Failure(Error.Failure("something went wrong")), cancellationToken: context.RequestAborted);
        }
    }
}
