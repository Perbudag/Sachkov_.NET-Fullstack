using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.SharedDto;
using Shared;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.TestData;

internal static class ApiTestData
{
    public static AddressDto Address(string street = "Кремлевская набережная", string house = "1") =>
        new("103132", "Россия", "Москва", "Москва", street, house, null);

    public static CreateLocationRequest LocationRequest(string name = "Московский Кремль", string street = "Кремлевская набережная", string house = "1") =>
        new(name, Address(street, house));

    public static CreateDepartmentRequest DepartmentRequest(
        string name = "Администрация",
        string slug = "administration",
        Guid? parentId = null,
        IEnumerable<Guid>? locationIds = null) =>
        new(name, slug, parentId, locationIds ?? []);

    public static CreatePositionRequest PositionRequest(string name = "Директор") => new(name);

}
