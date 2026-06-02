using System.Data.Common;

namespace DirectoryService.Infrastructure.Postgres.Database;

internal interface IDbConnectionFactory
{
    Task<DbConnection> CreateAsync(CancellationToken cancellationToken);
}