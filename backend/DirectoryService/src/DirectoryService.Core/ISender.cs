using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using Shared;

namespace DirectoryService.Core;

public interface ISender
{
    Task<Result<TResult, Failure>> SendAsync<TCommand, TResult>(ICommand<TCommand, TResult> command, CancellationToken cancellationToken) where TCommand : class, ICommand<TCommand, TResult>;
    Task<UnitResult<Failure>> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand;
    Task<Result<TResult, Failure>> SendAsync<TQuery, TResult>(IQuery<TQuery, TResult> query, CancellationToken cancellationToken) where TQuery : class, IQuery<TQuery, TResult>;
}
