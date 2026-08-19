using DirectoryService.Core.Abstractions.Database;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.Infrastructure.Postgres.Database;
using DirectoryService.Web.BackgroundServices.DatabaseCleaner;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace DirectoryService.IntegrationTests;

public class DirectoryTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18.4")
        .WithDatabase("dyrectory_service_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private DbConnection _connection;
    private Respawner _respawner = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<IDbConnectionFactory>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));


            services.AddSingleton<IDbConnectionFactory>(
                new DbConnectionFactory(_dbContainer.GetConnectionString()));

            services.Configure<DatabaseCleanerOptions>(options =>
            {
                options.DelayTime = TimeSpan.FromSeconds(5);
                options.AgeOfDeletion = TimeSpan.FromSeconds(5);
                options.BatchSize = 10;
            });
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();

        await using var scope = Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        _connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _connection.OpenAsync();

        await InitializeRespawner();
    }


    public new async ValueTask DisposeAsync()
    {
        await _connection.CloseAsync();
        await _connection.DisposeAsync();

        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();

        await base.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_connection);
    }


    private async Task InitializeRespawner()
    {
        _respawner = await Respawner.CreateAsync(_connection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"]
            }
        );
    }
}
