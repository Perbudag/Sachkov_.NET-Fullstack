
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure.Postgres;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructurePostgres(this IServiceCollection services, IConfigurationManager configurations)
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configurations.GetConnectionString("postgres")));

        return services;
    }
}
