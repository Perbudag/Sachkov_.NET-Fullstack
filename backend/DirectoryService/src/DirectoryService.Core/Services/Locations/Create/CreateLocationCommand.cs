using DirectoryService.Contracts.Locations;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Locations.Create;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand<CreateLocationCommand, Guid>
{
    public static implicit operator CreateLocationCommand(CreateLocationRequest request) => new(request);
}
