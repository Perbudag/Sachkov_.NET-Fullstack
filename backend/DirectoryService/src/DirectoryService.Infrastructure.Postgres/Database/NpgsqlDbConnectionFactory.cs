using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Data.Common;

namespace DirectoryService.Infrastructure.Postgres.Database;

internal class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgsqlDbConnectionFactory(IConfiguration configuration)
    {
        var dataStoreBuilder = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Postgresql"));

        dataStoreBuilder.UseLoggerFactory(CreateLoggerFactory(configuration));

        _dataSource = dataStoreBuilder.Build();
    }

    public async Task<DbConnection> CreateAsync(CancellationToken cancellationToken)
    {
        return await _dataSource.OpenConnectionAsync(cancellationToken);
    }

    private static ILoggerFactory CreateLoggerFactory(IConfiguration configuration) =>
        LoggerFactory.Create(builder => builder.AddConfiguration(configuration).AddConsole());
}
