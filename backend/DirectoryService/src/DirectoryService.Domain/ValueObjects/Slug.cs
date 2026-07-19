using CSharpFunctionalExtensions;
using Shared;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects
{
    public partial record Slug
    {
        public const int MIN_LENGTH = 2;
        public const int MAX_LENGTH = 100;

        private Slug(string value) => Value = value;

        public string Value { get; }

        public static Result<Slug, Failure> Create(string value)
        {
            if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
                return Error.Validation($"slug (от {MIN_LENGTH} до {MAX_LENGTH} символов)", "slug.validation.error").ToFailure();

            if (!SlugPattern.IsMatch(value))
                return Error.Validation("slug (только строчные латинские буквы, цифры и дефисы, " +
                    "не начинается и не заканчивается дефисом)", "slug.validation.error").ToFailure();

            return new Slug(value);
        }

        [GeneratedRegex(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
        private static partial Regex SlugPattern { get; }

        public override string ToString() => Value;
    }
}