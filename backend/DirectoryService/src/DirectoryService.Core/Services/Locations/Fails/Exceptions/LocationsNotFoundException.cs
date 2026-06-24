using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Locations.Fails.Exceptions;

public class LocationsNotFoundException : NotFoundException
{
    public LocationsNotFoundException(params IEnumerable<Error> errors) : base(errors)
    {
        
    }
}
