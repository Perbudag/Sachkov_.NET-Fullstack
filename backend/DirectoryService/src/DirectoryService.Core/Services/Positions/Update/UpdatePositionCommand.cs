using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Positions.Update;

public record UpdatePositionCommand(Guid Id, UpdatePositionRequest Request) : ICommand<UpdatePositionCommand, PositionDto>
{
    public static implicit operator UpdatePositionCommand((Guid, UpdatePositionRequest) args) => new(args.Item1, args.Item2);
}
