using Dapper;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Infrastructure.Postgres.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class DapperLocationsRepository : ILocationsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DapperLocationsRepository> _logger;

    public DapperLocationsRepository(IDbConnectionFactory connectionFactory, ILogger<DapperLocationsRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
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

        try
        {
            await connection.ExecuteAsync(sql, value);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to create location with id: {Id}", value.Id);
        }
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
