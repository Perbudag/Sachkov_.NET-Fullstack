using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace Shared.Framework.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services,
                                                       IConfigurationManager configuration,
                                                       string serviceName)
    {
        services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("ServiceName", serviceName));

        return services;
    }
}