using DirectoryService.Domain.ValueObjects;
using Shared;

namespace DirectoryService.Core.Fails;

public partial class Errors
{
    public static class DepartmentErrors
    {
        public static Error ValidationError(string message, string invalidField) =>
            Error.Validation(message, "departments.validation.error", invalidField);

        public static Error Conflict(string name) =>
            Error.Conflict($"A location named \"{name}\" already exists", "departments.is.conflict");

        public static Error LocationConflict() =>
            Error.Conflict("There is already such a location within the department", "departments.is.conflict.location");

        public static Error SlugConflict(Guid id, string slug) =>
            Error.Conflict($"The department with id \"{id}\" " +
                $"already has a child element with Slug \"{slug}\"", "departments.is.conflict");

        public static Error NotFoud() =>
            Error.NotFoud($"Department with this id not found", "departments.not.found");

        public static Error LocationNotFoud() =>
            Error.NotFoud($"There is no such location within the department", "departments.not.found.location");

    }
}
