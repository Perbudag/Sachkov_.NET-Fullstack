using DirectoryService.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests;

[Collection("Shared Fixture Collection")]
public abstract class DirectoryBaseTests : IAsyncLifetime
{
    private readonly IServiceProvider _services;
    private readonly Func<Task> _resetDatabase;


    protected HttpClient HttpClient { get; }


    protected DirectoryBaseTests(DirectoryTestWebFactory factory)
    {
        _services = factory.Services;
        _resetDatabase = factory.ResetDatabaseAsync;

        HttpClient = factory.CreateClient();
    }


    protected async Task<T> ExecuteInDbAsync<T>(Func<AppDbContext, Task<T>> action)
    {
        await using var scope = _services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        return await action(dbContext);
    }

    protected async Task ExecuteInDbAsync(Func<AppDbContext, Task> action)
    {
        await using var scope = _services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await action(dbContext);
    }

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync()
    {
        await _resetDatabase.Invoke();

        GC.SuppressFinalize(this);
    }
}
