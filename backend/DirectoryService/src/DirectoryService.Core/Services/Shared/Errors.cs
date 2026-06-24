using Shared;

namespace DirectoryService.Core.Fails;

public partial class Errors
{
    public static class SharedErrors
    {
        public static Error IsRequired(string invalidField, string? code = null) =>
            Error.Validation(invalidField + " is required", code, invalidField);
    }
}
