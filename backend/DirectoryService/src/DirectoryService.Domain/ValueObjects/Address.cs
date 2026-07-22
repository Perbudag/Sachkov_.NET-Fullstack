using CSharpFunctionalExtensions;
using Shared;
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
            errors.Add(Error.Validation("Почтовый индекс не может быть пустым.", "address.validation.error", nameof(postalCode)));

        if (country is null)
            errors.Add(Error.Validation("Название страны не может быть пустым.", "address.validation.error", nameof(country)));

        if (region is null)
            errors.Add(Error.Validation("Название региона не может быть пустым.", "address.validation.error", nameof(region)));

        if (city is null)
            errors.Add(Error.Validation("Название города не может быть пустым.", "address.validation.error", nameof(city)));

        if (street is null)
            errors.Add(Error.Validation("Название улицы не может быть пустым.", "address.validation.error", nameof(street)));

        if (house is null)
            errors.Add(Error.Validation("Номер дома не может быть пустым.", "address.validation.error", nameof(house)));


        if (postalCode != null && !PostalCodePattern.IsMatch(postalCode))
        {
            errors.Add(Error.Validation("Неправильный формат почтового индекса.", "address.validation.error", nameof(postalCode)));
        }

        if (country != null && !NamePattern.IsMatch(country))
        {
            errors.Add(Error.Validation("Название страны может состаять только" +
                " из букв и пробелов.", "address.validation.error", nameof(country)));
        }

        if (region != null && !NamePattern.IsMatch(region))
        {
            errors.Add(Error.Validation("Название региона может состаять только" +
                " из букв и пробелов.", "address.validation.error", nameof(region)));
        }

        if (city != null && !NamePattern.IsMatch(city))
        {
            errors.Add(Error.Validation("Название города может состаять только" +
                " из букв и пробелов.", "address.validation.error", nameof(city)));
        }

        if (street != null && !NamePattern.IsMatch(street))
        {
            errors.Add(Error.Validation("Название улицы может состаять только" +
                " из букв и пробелов.", "address.validation.error", nameof(street)));
        }

        if (house != null && !NumberPattern.IsMatch(house))
        {
            errors.Add(Error.Validation("Номер дома должен состоять из цифр" +
                "и/или прописных букв латинского и кириллического алфавита.", "address.validation.error", nameof(house)));
        }

        if (apartment != null && !NumberPattern.IsMatch(apartment))
        {
            errors.Add(Error.Validation("Номер квартиры должен состоять из цифр" +
                "и/или прописных букв латинского и кириллического алфавита.", "address.validation.error", nameof(apartment)));
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
                              .Replace(", ", "|", StringComparison.Ordinal)
                              .Split('|');

        if (valueParts.Length < 6)
        {
            return Error.Validation("Неправильный формат адреса. " +
                "Правильный формат выглядит так: \"{PostalCode}, {Country}, {Region}, г. {City}, ул. {Street}, д. {House}(_опционально_: \", кв. {Apartment}\")\"", "address.validation.error").ToFailure();
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