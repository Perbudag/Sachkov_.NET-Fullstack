using DirectoryService.Contracts.Locations;
using Shared.Core.CQRS;

namespace DirectoryService.Core.Services.Locations.GetById;

public record GetByIdLocationQuery(Guid Id) : IQuery<GetByIdLocationQuery, LocationDto>
{
    public static implicit operator GetByIdLocationQuery(Guid id) => new(id);
}
