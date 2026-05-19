using System.Globalization;
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


    public static Address Create(string postalCode,
                                 string country,
                                 string region,
                                 string city,
                                 string street,
                                 string house,
                                 string? apartment)
    {
        if (!PostalCodePattern.IsMatch(postalCode))
        {
            throw new ArgumentException("Неправильный формат почтового индекса.", nameof(postalCode));
        }

        if (!NamePattern.IsMatch(country))
        {
            throw new ArgumentException("Название страны может состаять только" +
                " из букв и пробелов.", nameof(country));
        }

        if (!NamePattern.IsMatch(region))
        {
            throw new ArgumentException("Название региона может состаять только" +
                " из букв и пробелов.", nameof(region));
        }

        if (!NamePattern.IsMatch(city))
        {
            throw new ArgumentException("Название города может состаять только" +
                " из букв и пробелов.", nameof(city));
        }

        if (!NamePattern.IsMatch(street))
        {
            throw new ArgumentException("Название улицы может состаять только" +
                " из букв и пробелов.", nameof(street));
        }

        if (!NumberPattern.IsMatch(house))
        {
            throw new ArgumentException("Номера дома должен состоять из цифр" +
                "и/или прописных букв латинского и кириллического алфавита.", nameof(house));
        }

        if (apartment != null && !NumberPattern.IsMatch(apartment))
        {
            throw new ArgumentException("Номера квартиры должен состоять из цифр" +
                "и/или прописных букв латинского и кириллического алфавита.", nameof(apartment));
        }

        return new Address(postalCode, country, region, city, street, house, apartment);
    }

    public static Address Create(string value)
    {
        var valueParts = value.Replace(", ", "|", StringComparison.Ordinal)
                              .Replace(", г. ", "|", StringComparison.Ordinal)
                              .Replace(", ул. ", "|", StringComparison.Ordinal)
                              .Replace(", д. ", "|", StringComparison.Ordinal)
                              .Split('|');

        if (valueParts.Length < 6)
        {
            throw new ArgumentException("Неправильный формат адреса. " +
                "Правильный формат выглядит так: \"{PostalCode}, {Country}, {Region}, г. {City}, ул. {Street}, д. {House}(_опционально_: \", кв. {Apartment}\")\"",
                nameof(value));
        }

        return new Address(postalCode: valueParts[0].Trim(),
                           country: valueParts[1].Trim(),
                           region: valueParts[2].Trim(),
                           city: valueParts[3].Trim(),
                           street: valueParts[4].Trim(),
                           house: valueParts[5].Trim(),
                           apartment: valueParts.Length == 7 ? valueParts[6].Trim() : null);
    }


    [GeneratedRegex(@"^[0-9|A-Z| |-]{4,8}$", RegexOptions.Compiled)]
    private static partial Regex PostalCodePattern { get; }


    [GeneratedRegex(@"^[а-я|А-Я|a-z|A-Z| ]$", RegexOptions.Compiled)]
    private static partial Regex NamePattern { get; }


    [GeneratedRegex(@"^[0-9|а-я|a-z]$", RegexOptions.Compiled)]
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