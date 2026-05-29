using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record LocationResponse(Guid Id, string Name, AddressDto Address);