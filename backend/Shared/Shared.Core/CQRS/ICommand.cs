namespace Shared.Core.CQRS;

public interface ICommand;
public interface ICommand<TSelf, out TResult> where TSelf : class, ICommand<TSelf, TResult>;