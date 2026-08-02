
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Positions.Delete;

public record DeletePositionCommand(Guid Id) : ICommand
{
    public static implicit operator DeletePositionCommand(Guid id) => new(id);
}
