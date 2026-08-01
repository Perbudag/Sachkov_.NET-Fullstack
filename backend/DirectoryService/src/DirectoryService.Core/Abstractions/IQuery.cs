namespace DirectoryService.Core.Abstractions;

#pragma warning disable CA1040 // Не используйте пустые интерфейсы
public interface IQuery<TSelf, out TResult> where TSelf : class, IQuery<TSelf, TResult>;
#pragma warning restore CA1040 // Не используйте пустые интерфейсы