using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Services.Locations.GetById;

public record GetByIdLocationQuery(Guid Id) : IQuery<GetByIdLocationQuery, LocationDto>
{
    public static implicit operator GetByIdLocationQuery(Guid id) => new(id);
}
