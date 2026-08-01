using CSharpFunctionalExtensions;
using DirectoryService.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Shared;

namespace DirectoryService.Core;

internal class Sender : ISender
{
    private readonly IServiceProvider _serviceProvider;

    public Sender(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task<Result<TResult, Failure>> SendAsync<TCommand, TResult>(ICommand<TCommand, TResult> command, CancellationToken cancellationToken) where TCommand : class, ICommand<TCommand, TResult>
        => _serviceProvider.GetRequiredService<ICommandHandler<TResult, TCommand>>().HandleAsync((TCommand)command, cancellationToken);

    public Task<UnitResult<Failure>> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : class, ICommand
        => _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>().HandleAsync(command, cancellationToken);

    public Task<Result<TResult, Failure>> SendAsync<TQuery, TResult>(IQuery<TQuery, TResult> query, CancellationToken cancellationToken) where TQuery : class, IQuery<TQuery, TResult>
        => _serviceProvider.GetRequiredService<IQueryHandler<TResult, TQuery>>().HandleAsync((TQuery)query, cancellationToken);
}
