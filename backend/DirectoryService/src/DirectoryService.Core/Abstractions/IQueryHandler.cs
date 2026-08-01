using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Abstractions;

public interface IQueryHandler<TResult, in TQuery> where TQuery: class, IQuery<TQuery, TResult>
{
    Task<Result<TResult, Failure>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}