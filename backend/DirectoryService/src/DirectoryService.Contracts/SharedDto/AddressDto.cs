namespace DirectoryService.Contracts.SharedDto;

public record AddressDto(string PostalCode, string Country, string Region, string City, string Street, string House, string? Apartment);
