using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Core.CQRS;

namespace DirectoryService.Core;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfigurationManager configurations)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionExtensions).Assembly);

        var assembly = typeof(DependencyInjectionExtensions).Assembly;
        services.AddSqrs(assembly);

        return services;
    }
}
