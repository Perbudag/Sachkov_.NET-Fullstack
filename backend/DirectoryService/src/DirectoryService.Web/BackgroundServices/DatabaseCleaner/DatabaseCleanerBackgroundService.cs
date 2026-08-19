using DirectoryService.Core.Abstractions.Repositories;
using Microsoft.Extensions.Options;

namespace DirectoryService.Web.BackgroundServices.DatabaseCleaner;

public class DatabaseCleanerBackgroundService : BackgroundService
{
    private readonly DatabaseCleanerOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DatabaseCleanerBackgroundService> _logger;

    public DatabaseCleanerBackgroundService(ILogger<DatabaseCleanerBackgroundService> logger,
                                            IServiceScopeFactory scopeFactory,
                                            IOptions<DatabaseCleanerOptions> options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateAsyncScope();
            var respository = scope.ServiceProvider.GetRequiredService<IClearDatabaseRepository>();

            _logger.LogInformation("Background service is running at: {Time}", DateTime.Now);

            var deletedCount = await respository.CleanAsync(_options.AgeOfDeletion, _options.BatchSize, stoppingToken);

            _logger.LogInformation("Background service deleted {Count} entities.", deletedCount);

            await Task.Delay(_options.DelayTime, stoppingToken);
        }
    }
}
