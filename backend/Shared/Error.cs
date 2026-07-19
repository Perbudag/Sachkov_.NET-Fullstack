using System.Security.Cryptography.X509Certificates;

namespace Shared;

public record Error(
        string Code,
        string Message,
        ErrorType Type,
        string? InvalidField = null
    )
{
    public static Error Validation(string message, string? code = null, string? invalidField = null) =>
        new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error NotFoud(string message, string? code = null, Guid? id = null) =>
        new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);

    public static Error Conflict(string message, string? code = null) =>
        new(code ?? "record.is.conflict", message, ErrorType.CONFLICT);

    public static Error Failure(string message, string? code = null) =>
        new(code ?? "failure", message, ErrorType.FAILURE);


    public Failure ToFailure() => this;
}
