using DirectoryService.Core.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfigurationManager configurations)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);
        services.AddScoped<ISender, Sender>();

        var assembly = typeof(DependencyInjectionExtensions).Assembly;

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(IQueryHandler<,>)), false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
