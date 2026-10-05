using CSharpFunctionalExtensions;
using Shared.Kernel;

namespace Shared.Core.CQRS;

public interface IQueryHandler<TResult, in TQuery> where TQuery: class, IQuery<TQuery, TResult>
{
    Task<Result<TResult, Failure>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}