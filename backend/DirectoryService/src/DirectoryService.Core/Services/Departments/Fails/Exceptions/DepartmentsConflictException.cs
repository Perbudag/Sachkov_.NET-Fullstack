using DirectoryService.Core.Exceptions;
using Shared;

namespace DirectoryService.Core.Services.Departments.Fails.Exceptions;

public class DepartmentsConflictException : ConflictException
{
    public DepartmentsConflictException(params IEnumerable<Error> errors) : base(errors)
    {

    }
}
