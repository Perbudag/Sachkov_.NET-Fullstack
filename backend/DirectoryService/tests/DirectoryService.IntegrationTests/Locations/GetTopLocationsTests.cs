using DirectoryService.Contracts.Locations;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class GetTopLocationsTests : DirectoryBaseTests
{
    public GetTopLocationsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_top_locations_With_related_department_Should_return_location()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var locationId = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            await ExecuteInDbAsync(db => DbTestData.CreateDepartmentLocationAsync(db, departmentId, locationId, cancellationToken));
    
            // act
            var response = await HttpClient.GetAsync("api/Locations/top", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationListItemDto[]>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Contains(envelope!.Result!, location => location.Id == locationId && location.DepartmentCount == 1);
        }
}
