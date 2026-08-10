using DirectoryService.Contracts.SharedDto;

namespace DirectoryService.Contracts.Locations;

public record TopLocationDto(Guid Id, string Name, AddressDto Address, int DepartmentCount);