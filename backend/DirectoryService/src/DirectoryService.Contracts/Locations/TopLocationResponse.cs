using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record TopLocationResponse(Guid Id, string Name, AddressDto Address, int DepartmentCount);