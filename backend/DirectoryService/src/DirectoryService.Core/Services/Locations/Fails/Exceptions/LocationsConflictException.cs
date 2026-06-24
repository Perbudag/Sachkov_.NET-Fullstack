using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Locations.Fails.Exceptions;

public class LocationsConflictException : ConflictException
{
    public LocationsConflictException(params IEnumerable<Error> errors) : base(errors)
    {

    }
}
