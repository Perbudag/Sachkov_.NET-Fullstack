using System.Data;

namespace DirectoryService.Core.Abstractions.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateAsync(CancellationToken cancellationToken);
}
