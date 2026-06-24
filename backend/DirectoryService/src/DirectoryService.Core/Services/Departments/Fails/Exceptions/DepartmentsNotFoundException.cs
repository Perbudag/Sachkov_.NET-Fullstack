using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Departments.Fails.Exceptions;

public class DepartmentsNotFoundException : NotFoundException
{
    public DepartmentsNotFoundException(params IEnumerable<Error> errors) : base(errors)
    {
        
    }
}
