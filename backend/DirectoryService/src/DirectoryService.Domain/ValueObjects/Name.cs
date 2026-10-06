using CSharpFunctionalExtensions;
using DirectoryService.Domain.Errors;
using Shared.Kernel;

namespace DirectoryService.Domain.ValueObjects;

public record Name
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 150;

    private Name(string value) => Value = value;

    public string Value { get; }

    public static Result<Name, Failure> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return SharedErrors.IsRequired("Name", "name.validation.error").ToFailure();
        }

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
        {
            return SharedErrors.InvalidLength("Name", MIN_LENGTH, MAX_LENGTH, "name.validation.error").ToFailure();
        }

        return new Name(value);
    }

    public override string ToString() => Value;
}