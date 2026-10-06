namespace Shared.Core.CQRS;

public interface IQuery<TSelf, out TResult> where TSelf : class, IQuery<TSelf, TResult>;