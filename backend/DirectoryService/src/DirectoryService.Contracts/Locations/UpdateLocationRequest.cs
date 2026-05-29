using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record UpdateLocationRequest(Guid Id, string Name, AddressDto Addess);
