using DirectoryService.Core.Services.Departments;
using DirectoryService.Core.Services.Locations;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfigurationManager configurations)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);

        services.AddScoped<ILocationsService, LocationsService>();
        services.AddScoped<IDepartmentsService, DepartmentsService>();

        return services;
    }
}
