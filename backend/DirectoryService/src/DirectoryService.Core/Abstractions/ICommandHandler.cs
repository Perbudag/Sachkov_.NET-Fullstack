using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Core.Abstractions;


public interface ICommandHandler<TResult, in TCommand> where TCommand : class, ICommand<TCommand, TResult>
{
    Task<Result<TResult, Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<UnitResult<Failure>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}