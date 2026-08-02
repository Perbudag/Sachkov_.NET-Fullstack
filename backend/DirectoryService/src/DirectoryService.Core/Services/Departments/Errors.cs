using Shared;

namespace DirectoryService.Core.Fails;

public partial class Errors
{
    public static class DepartmentErrors
    {
        public static Error ValidationError(string message, string invalidField) =>
            Error.Validation(message, "departments.validation.error", invalidField);

        public static Error Conflict(string name) =>
            Error.Conflict($"A department named \"{name}\" already exists", "departments.is.conflict");

        public static Error LocationConflict() =>
            Error.Conflict("There is already such a location within the department", "departments.is.conflict.location");

        public static Error SlugConflict(Guid id, string slug) =>
            Error.Conflict($"The department with id \"{id}\" " +
                $"already has a child element with Slug \"{slug}\"", "departments.is.conflict");

        public static Error NotFoud() =>
            Error.NotFoud($"Department with this id not found", "departments.not.found");

        public static Error NotFoudName() =>
            Error.NotFoud($"Department with this name not found", "departments.not.found");
        

        public static Error NotFoudParent() =>
            Error.NotFoud($"Parent department with this identifier was not found.", "departments.not.found.parent");

        public static Error LocationNotFound() =>
            Error.NotFoud($"There is no such location within the department", "departments.not.found.location");


        public static Error LocationNotFoudMany(IEnumerable<Guid> ids) =>
            Error.NotFoud("Locations with this ids not found: " + string.Join(", ", ids), "departments.not.found.locations");

        public static Error PositionNotFound() =>
            Error.NotFoud($"There is no such position within the department", "departments.not.found.position");

        public static Error PositionConflict() =>
            Error.Conflict("There is already such a position within the department", "departments.is.conflict.position");
    }
}
