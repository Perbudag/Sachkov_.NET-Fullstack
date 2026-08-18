using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Abstractions.Repositories;

public interface IClearDatabaseRepository
{
    Task<int> CleanAsync(TimeSpan ageOfDeletion, int batchSize, CancellationToken cancellationToken);
}
