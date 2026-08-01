using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Locations.Create;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand<CreateLocationCommand, Guid>
{
    public static implicit operator CreateLocationCommand(CreateLocationRequest request) => new(request);
}
