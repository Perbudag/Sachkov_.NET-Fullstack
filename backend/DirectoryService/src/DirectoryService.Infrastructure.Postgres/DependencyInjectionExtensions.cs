using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Core.Services.Departments;
using DirectoryService.Core.Services.Locations;
using DirectoryService.Core.Services.Positions;
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
        services.AddScoped<ILocationsRepository, LocationRepository>();
        services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
        services.AddScoped<IPositionsRepository, PositionsRepository>();

        services.AddScoped<ITransactionManager, TransactionManager>();

        services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(configurations.GetConnectionString("Postgresql")));

        services.AddScoped<IReadDbContext>(sp => sp.GetRequiredService<AppDbContext>());
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
