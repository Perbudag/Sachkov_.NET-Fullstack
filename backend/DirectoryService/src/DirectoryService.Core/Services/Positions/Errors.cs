using Shared;

namespace DirectoryService.Core.Fails;

public static partial class Errors
{

    public static class PositionsErrors
    {
        public static Error ValidationError(string message, string invalidField) =>
            Error.Validation(message, "positions.validation.error", invalidField);

        public static Error ConflictName(string name) =>
            Error.Conflict($"A position named \"{name}\" already exists", "positions.is.conflict");

        public static Error NotFoud() =>
            Error.NotFoud($"Position with this id not found", "positions.not.found");

        public static Error NotFoudName() =>
            Error.NotFoud($"Position with this name not found", "positions.not.found");
    }
}
