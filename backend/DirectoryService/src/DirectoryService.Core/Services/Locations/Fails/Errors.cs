using DirectoryService.Domain.Entities;
using Shared;

namespace DirectoryService.Core.Fails;

public static partial class Errors
{

    public static class LocationErrors
    {
        public static Error ValidationError(string message, string invalidField) =>
            Error.Validation(message, "locations.validation.error", invalidField);

        public static Error ConflictName(string name) =>
            Error.Conflict($"A location named \"{name}\" already exists", "locations.is.conflict");

        public static Error NotFoud() =>
            Error.NotFoud($"Location with this id not found", "locations.not.found");
        public static Error NotFoudName() =>
            Error.NotFoud($"Location with this name not found", "locations.not.found");

        public static Error NotFoudMany(IEnumerable<Guid> ids) =>
            Error.NotFoud("Locations with this ids not found: " + string.Join(", ", ids), "departments.not.found");
    }
}
