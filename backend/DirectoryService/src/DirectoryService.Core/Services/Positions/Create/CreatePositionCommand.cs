using DirectoryService.Contracts.Positions;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Positions.Create;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand<CreatePositionCommand, Guid>
{
    public static implicit operator CreatePositionCommand(CreatePositionRequest request) => new(request);
}
