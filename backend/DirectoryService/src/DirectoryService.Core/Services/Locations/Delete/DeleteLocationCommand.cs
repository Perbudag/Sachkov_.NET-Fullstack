using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Locations.Delete;

public record DeleteLocationCommand(Guid Id) : ICommand
{
    public static implicit operator DeleteLocationCommand(Guid id) => new(id);
}
