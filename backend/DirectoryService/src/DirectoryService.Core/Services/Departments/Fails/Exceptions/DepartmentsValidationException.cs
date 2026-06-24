using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Departments.Fails.Exceptions;

public class DepartmentsValidationException : BadRequestException
{
    public DepartmentsValidationException(params IEnumerable<Error> errors) : base(errors)
    {
        
    }
}
