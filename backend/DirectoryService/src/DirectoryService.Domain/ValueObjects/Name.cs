using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects;

public record Name
{
    public const int MIN_LENGTH = 2;
    public const int MAX_LENGTH = 150;

    private Name(string value) => Value = value;

    public string Value { get; }

    public static Name Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
               $"name не может быть пустым",
               nameof(value));
        }

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            throw new ArgumentException(
               $"name (от {MIN_LENGTH} до {MAX_LENGTH} символов)",
               nameof(value));

        return new Name(value);
    }

    public override string ToString() => Value;
}