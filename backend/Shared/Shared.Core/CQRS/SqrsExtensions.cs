using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Shared.Core.CQRS;

public static class SqrsExtensions
{
    public static IServiceCollection AddSqrs(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<ISender, Sender>();

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
