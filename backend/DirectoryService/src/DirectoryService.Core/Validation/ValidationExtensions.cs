using FluentValidation.Results;
using Shared;

namespace DirectoryService.Core.Validation;

public static class ValidationExtensions
{
    public static Failure ToErrors(this ValidationResult result) =>
       new Failure(result.Errors.Select(ve => new Error(ve.ErrorCode, ve.ErrorMessage, (ErrorType?)ve.CustomState ?? ErrorType.VALIDATION, ve.PropertyName)));
}
