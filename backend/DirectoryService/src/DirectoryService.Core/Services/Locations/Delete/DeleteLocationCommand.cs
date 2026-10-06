using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Locations.Delete;

public record DeleteLocationCommand(Guid Id) : ICommand
{
    public static implicit operator DeleteLocationCommand(Guid id) => new(id);
}
