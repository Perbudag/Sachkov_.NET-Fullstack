using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Abstractions;

public interface IQueryHandler<TResult, in TQuery> where TQuery: IQuery
{
    Task<Result<TResult, Failure>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}