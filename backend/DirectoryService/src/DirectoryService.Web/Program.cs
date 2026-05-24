using DirectoryService.Infrastructure.Postgres;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructurePostgres(builder.Configuration);

var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.UseHealthChecks("/health");


if(!app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


await app.RunAsync();