using CSharpFunctionalExtensions;
using DirectoryService.Domain.Errors;
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
            {
                return SharedErrors.InvalidLength("Slug", MIN_LENGTH, MAX_LENGTH, "slug.validation.error").ToFailure();
            }

            if (!SlugPattern.IsMatch(value))
            {
                return SharedErrors.InvalidFormat(
                    "Slug",
                    "only lowercase letters, digits, and hyphens; cannot start or end with a hyphen.",
                    "slug.validation.error"
                ).ToFailure();
            }

            return new Slug(value);
        }

        [GeneratedRegex(@"^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
        private static partial Regex SlugPattern { get; }

        public override string ToString() => Value;
    }
}