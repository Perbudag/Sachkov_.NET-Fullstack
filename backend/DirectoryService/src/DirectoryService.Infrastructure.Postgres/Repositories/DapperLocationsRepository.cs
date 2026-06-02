using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Infrastructure.Postgres.Database;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class DapperLocationsRepository : ILocationsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperLocationsRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateAsync(cancellationToken);

        var sql = """
            INSERT INTO 
              locations (
                location_id, 
                name, 
                address, 
                created_at, 
                updated_at
              )
            VALUES
              (
                @Id, 
                @Name, 
                @Address, 
                @CreatedAt, 
                @UpdatedAt
              );
            """;

        var value = new
        {
            Id = location.Id,
            Name = location.Name.ToString(),
            Address = location.Address.ToString(),
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt,
        };

        await connection.ExecuteAsync(sql, value);
    }

    public async Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken)
    {
        using var connection = await _connectionFactory.CreateAsync(cancellationToken);

        var sql = """
            SELECT EXISTS (
                SELECT location_id
                FROM locations 
                WHERE name = @Name
            );
            """;

        var value = new
        {
            Name = name.ToString(),
        };

        return await connection.QuerySingleAsync<bool>(sql, value);
    }
}
