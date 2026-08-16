using DirectoryService.Core.Abstractions.Database;
using Npgsql;
using System.Data;

namespace DirectoryService.Infrastructure.Postgres.Database;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
