using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Abstractions.Database;

public interface ITransactionScope : IDisposable
{
    UnitResult<Error> Commit();
    UnitResult<Error> Rollback();
}