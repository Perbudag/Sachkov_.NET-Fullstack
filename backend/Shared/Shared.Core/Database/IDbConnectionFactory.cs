using System.Data;

namespace Shared.Core.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateAsync(CancellationToken cancellationToken);
}
