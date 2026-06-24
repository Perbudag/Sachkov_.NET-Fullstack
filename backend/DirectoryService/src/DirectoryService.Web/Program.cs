using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructurePostgres(builder.Configuration);
builder.Services.AddCore(builder.Configuration);

builder.Services.AddScoped<ExceptionMiddleware>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.UseHealthChecks("/health");


if(!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


await app.RunAsync();