using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Infrastructure.Postgres.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfigurationManager configurations)
    {
        services.AddSingleton<IDbConnectionFactory, NpgsqlDbConnectionFactory>();

        services.AddScoped<ILocationsRepository, LocationRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();

        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configurations.GetConnectionString("Postgresql"))
                                    .UseLoggerFactory(LoggerFactory.Create(builder =>
                                    {
                                        builder.AddConfiguration(configurations);
                                        builder.AddConsole();
                                    })));

        return services;
    }
}
