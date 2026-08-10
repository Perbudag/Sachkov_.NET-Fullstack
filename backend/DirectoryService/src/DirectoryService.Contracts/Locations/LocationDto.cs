using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record LocationDto(Guid Id, string Name, AddressDto Address);