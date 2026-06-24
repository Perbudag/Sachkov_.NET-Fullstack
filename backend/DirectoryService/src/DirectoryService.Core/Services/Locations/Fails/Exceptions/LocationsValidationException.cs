using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Locations.Fails.Exceptions;

public class LocationsValidationException : BadRequestException
{
    public LocationsValidationException(params IEnumerable<Error> errors) : base(errors)
    {
        
    }
}
