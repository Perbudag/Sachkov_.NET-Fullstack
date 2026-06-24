using DirectoryService.Core.Exceptions;
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
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
#pragma warning disable CA2254 // Шаблон должен быть статическим выражением
        _logger.LogError(exception, exception.Message);
#pragma warning restore CA2254 // Шаблон должен быть статическим выражением

        var (code, errors) = exception switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),

            NotFoundException => (StatusCodes.Status404NotFound, JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),

            ConflictException => (StatusCodes.Status409Conflict, JsonSerializer.Deserialize<IEnumerable<Error>>(exception.Message)),

            _ => (StatusCodes.Status500InternalServerError, [Error.Failure("something went wrong")])
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsJsonAsync(errors);
    }
}
