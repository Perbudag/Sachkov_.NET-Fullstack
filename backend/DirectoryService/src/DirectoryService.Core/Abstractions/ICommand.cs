namespace DirectoryService.Core.Abstractions;

#pragma warning disable CA1040 // Не используйте пустые интерфейсы

#pragma warning disable S2326 // Unused type parameters should be removed
public interface ICommand;
public interface ICommand<TSelf, out TResult> where TSelf : class, ICommand<TSelf, TResult>;
#pragma warning restore S2326 // Unused type parameters should be removed

#pragma warning restore CA1040 // Не используйте пустые интерфейсы