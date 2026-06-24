using DirectoryService.Core.Fails;
using FluentValidation.Results;
using Shared;

namespace DirectoryService.Core.Extensions;

public static class ValidationExtensions
{
    public static IEnumerable<Error> ToErrors(this ValidationResult result, Func<string, string, Error> errorFactory) =>
        result.Errors.Select(e => errorFactory(e.ErrorMessage, e.PropertyName));
}
