using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record CreateLocationRequest(string Name, AddressDto Address);
