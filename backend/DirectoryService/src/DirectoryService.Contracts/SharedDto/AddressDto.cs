using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DirectoryService.Contracts.SharedDto;

public record AddressDto(string PostalCode, string Country, string Region, string City, string Street, string House, string? Apartment)
{
    public static AddressDto Parse(string row)
    {
        var rowParts = row.Replace(", г. ", "|", StringComparison.Ordinal)
                          .Replace(", ул. ", "|", StringComparison.Ordinal)
                          .Replace(", д. ", "|", StringComparison.Ordinal)
                          .Replace(", ", "|", StringComparison.Ordinal)
                          .Split('|');

        return new AddressDto(PostalCode: rowParts[0].Trim(),
                              Country: rowParts[1].Trim(),
                              Region: rowParts[2].Trim(),
                              City: rowParts[3].Trim(),
                              Street: rowParts[4].Trim(),
                              House: rowParts[5].Trim(),
                              Apartment: rowParts.Length == 7 ? rowParts[6].Trim() : null);
    }

    public static implicit operator AddressDto(string row) => AddressDto.Parse(row);
}
