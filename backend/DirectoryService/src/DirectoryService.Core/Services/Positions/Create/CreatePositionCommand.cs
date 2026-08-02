using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Positions.Create;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand<CreatePositionCommand, Guid>
{
    public static implicit operator CreatePositionCommand(CreatePositionRequest request) => new(request);
}
