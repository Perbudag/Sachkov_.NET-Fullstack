using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

internal class ClearDatabaseRepository : IClearDatabaseRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClearDatabaseRepository> _logger;

    public ClearDatabaseRepository(AppDbContext context, ILogger<ClearDatabaseRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> CleanAsync(TimeSpan ageOfDeletion, int batchSize, CancellationToken cancellationToken)
    {
        DateTime thresholdDate = DateTime.UtcNow - ageOfDeletion;

        int totalDeletedCount = 0;
        var departmentsDeletedCount = int.MaxValue;
        var locationsDeletedCount = int.MaxValue;
        var positionsDeletedCount = int.MaxValue;

        try
        {

            while (!cancellationToken.IsCancellationRequested
                    && (departmentsDeletedCount >= batchSize
                    || locationsDeletedCount >= batchSize
                    || positionsDeletedCount >= batchSize))
            {
                if (departmentsDeletedCount >= batchSize)
                {
                    departmentsDeletedCount = await _context.Departments
                         .Where(d => d.IsDeleted && d.DeletedAt <= thresholdDate)
                         .Take(batchSize)
                         .ExecuteDeleteAsync(cancellationToken);

                    totalDeletedCount += departmentsDeletedCount;
                }


                if (locationsDeletedCount >= batchSize)
                {
                    locationsDeletedCount = await _context.Locations
                         .Where(l => l.IsDeleted && l.DeletedAt <= thresholdDate)
                         .Take(batchSize)
                         .ExecuteDeleteAsync(cancellationToken);

                    totalDeletedCount += locationsDeletedCount;
                }


                if (positionsDeletedCount >= batchSize)
                {
                    positionsDeletedCount = await _context.Positions
                         .Where(p => p.IsDeleted && p.DeletedAt <= thresholdDate)
                         .Take(batchSize)
                         .ExecuteDeleteAsync(cancellationToken);

                    totalDeletedCount += positionsDeletedCount;
                }
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Something went wrong when cleaning the database.");
        }

        return totalDeletedCount;
    }
}
