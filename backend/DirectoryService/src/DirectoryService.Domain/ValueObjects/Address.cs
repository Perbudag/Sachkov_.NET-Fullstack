using CSharpFunctionalExtensions;
using DirectoryService.Domain.Errors;
using Shared.Kernel;
using System.Text;
using System.Text.RegularExpressions;

namespace DirectoryService.Domain.ValueObjects;

public partial record Address
{
    private Address(string postalCode, string country, string region, string city, string street, string house, string? apartment)
    {
        PostalCode = postalCode;
        Country = country;
        Region = region;
        City = city;
        Street = street;
        House = house;
        Apartment = apartment;
    }


    public string PostalCode { get; }  // Почтовый индекс (например, 367010)
    public string Country { get; }     // Страна (например, Россия)
    public string Region { get; }      // Регион / Область (например, Республика Дагестан)
    public string City { get; }        // Город / Населенный пункт (например, Махачкала)
    public string Street { get; }      // Улица (например, проспект Петра I)
    public string House { get; }       // Дом (например, 51)
    public string? Apartment { get; }  // Квартира / Офис (например, 42)


    public static Result<Address, Failure> Create(string postalCode,
                                 string country,
                                 string region,
                                 string city,
                                 string street,
                                 string house,
                                 string? apartment)
    {
        var errors = new List<Error>();

        if (postalCode is null)
            errors.Add(SharedErrors.IsRequired(nameof(postalCode), "address.validation.error"));
        
        if (country is null)
            errors.Add(SharedErrors.IsRequired(nameof(country), "address.validation.error"));

        if (region is null)
            errors.Add(SharedErrors.IsRequired(nameof(region), "address.validation.error"));

        if (city is null)
            errors.Add(SharedErrors.IsRequired(nameof(city), "address.validation.error"));

        if (street is null)
            errors.Add(SharedErrors.IsRequired(nameof(street), "address.validation.error"));

        if (house is null)
            errors.Add(SharedErrors.IsRequired(nameof(house), "address.validation.error"));


        if (postalCode != null && !PostalCodePattern.IsMatch(postalCode))
        {
            errors.Add(SharedErrors.InvalidFormat("Postal code", "4 to 8 digits, letters, or hyphens.", "address.validation.error"));
        }

        if (country != null && !NamePattern.IsMatch(country))
        {
            errors.Add(SharedErrors.InvalidFormat("Country", "letters and spaces only.", "address.validation.error"));
        }

        if (region != null && !NamePattern.IsMatch(region))
        {
            errors.Add(SharedErrors.InvalidFormat("Region", "letters and spaces only.", "address.validation.error"));
        }

        if (city != null && !NamePattern.IsMatch(city))
        {
            errors.Add(SharedErrors.InvalidFormat("City", "letters and spaces only.", "address.validation.error"));
        }

        if (street != null && !NamePattern.IsMatch(street))
        {
            errors.Add(SharedErrors.InvalidFormat("Street", "letters and spaces only.", "address.validation.error"));
        }

        if (house != null && !NumberPattern.IsMatch(house))
        {
            errors.Add(SharedErrors.InvalidFormat("House", "digits and/or letters.", "address.validation.error"));
        }

        if (!string.IsNullOrWhiteSpace(apartment) && !NumberPattern.IsMatch(apartment))
        {
            errors.Add(SharedErrors.InvalidFormat("Apartment", "digits and/or letters.", "address.validation.error"));
        }

        if (errors.Count > 0)
            return new Failure(errors);

        return new Address(postalCode!, country!, region!, city!, street!, house!, apartment);
    }

    public static Result<Address, Failure> Create(string value)
    {
        var valueParts = value.Replace(", г. ", "|", StringComparison.Ordinal)
                              .Replace(", ул. ", "|", StringComparison.Ordinal)
                              .Replace(", д. ", "|", StringComparison.Ordinal)
                              .Replace(", кв. ", "|", StringComparison.Ordinal)
                              .Replace(", ", "|", StringComparison.Ordinal)
                              .Split('|');

        if (valueParts.Length < 6)
        {
            return SharedErrors.InvalidStringStructure(
                "Address",
                "{PostalCode}, {Country}, {Region}, г. {City}, ул. {Street}, д. {House} [, кв. {Apartment}]",
                "address.validation.error"
            ).ToFailure();
        }

        return Address.Create(postalCode: valueParts[0].Trim(),
                           country: valueParts[1].Trim(),
                           region: valueParts[2].Trim(),
                           city: valueParts[3].Trim(),
                           street: valueParts[4].Trim(),
                           house: valueParts[5].Trim(),
                           apartment: valueParts.Length == 7 ? valueParts[6].Trim() : null);
    }


    [GeneratedRegex(@"^[0-9|A-Z| |-]{4,8}$", RegexOptions.Compiled)]
    private static partial Regex PostalCodePattern { get; }


    [GeneratedRegex(@"^[а-я|А-Я|a-z|A-Z| ]+$", RegexOptions.Compiled)]
    private static partial Regex NamePattern { get; }


    [GeneratedRegex(@"^[0-9|а-я|a-z]+$", RegexOptions.Compiled)]
    private static partial Regex NumberPattern { get; }

    public override string ToString()
    {
        var result = new StringBuilder();

        result.Append(PostalCode);
        result.Append(", ");
        result.Append(Country);
        result.Append(", ");
        result.Append(Region);
        result.Append(", г. ");
        result.Append(City);
        result.Append(", ул. ");
        result.Append(Street);
        result.Append(", д. ");
        result.Append(House);

        if (Apartment != null)
        {
            result.Append(", кв. ");
            result.Append(Apartment);
        }

        return result.ToString();
    }
}