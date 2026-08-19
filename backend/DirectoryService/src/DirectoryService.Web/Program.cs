using DirectoryService.Core;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Web.BackgroundServices.DatabaseCleaner;
using DirectoryService.Web.Middlewares;
using Scalar.AspNetCore;
using Serilog;
using System.Globalization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);


    builder.Services.AddOpenApi();

    builder.Services.AddControllers();

    builder.Services.AddHealthChecks();

    builder.Services.AddInfrastructurePostgres(builder.Configuration);
    builder.Services.AddCore(builder.Configuration);

    builder.Services.AddScoped<ExceptionMiddleware>();

    builder.Services.AddSerilog((services, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.WithProperty("ServiceName", "DirectoryService"));

    builder.Services.AddHostedService<DatabaseCleanerBackgroundService>();

    builder.Services.Configure<DatabaseCleanerOptions>(
        builder.Configuration.GetSection(DatabaseCleanerOptions.SectionName));

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseMiddleware<ExceptionMiddleware>();

    app.MapControllers();

    app.UseHealthChecks("/health");


    if (!app.Environment.IsProduction())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    await app.RunAsync();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

namespace DirectoryService.Web
{
    public partial class Program;
}