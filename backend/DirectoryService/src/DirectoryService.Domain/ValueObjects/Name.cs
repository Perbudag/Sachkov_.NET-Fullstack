using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.ValueObjects;

public record Name
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 150;

    private Name(string value) => Value = value;

    public string Value { get; }

    public static Result<Name, Failure> Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("name не может быть пустым", "name.validation.error").ToFailure();
        }

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return Error.Validation($"name (от {MIN_LENGTH} до {MAX_LENGTH} символов)", "name.validation.error").ToFailure();

        return new Name(value);
    }

    public override string ToString() => Value;
}