using DirectoryService.Contracts.Locations;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Locations.Update;

public record UpdateLocationCommand(Guid Id, UpdateLocationRequest Request) : ICommand<UpdateLocationCommand, LocationDto>
{
    public static implicit operator UpdateLocationCommand((Guid, UpdateLocationRequest) args) => 
        new(args.Item1, args.Item2);
}