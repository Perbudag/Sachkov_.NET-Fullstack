using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class RemoveLocationFromDepartmentTests : DirectoryBaseTests
{
    public RemoveLocationFromDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Remove_location_from_department_When_relation_missing_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var locationId = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
    
            // act
            var response = await HttpClient.DeleteAsync($"api/Departments/{departmentId}/locations/{locationId}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found.location", StringComparison.Ordinal));
        }

    [Fact]
    public async Task Remove_location_from_department_With_existing_relation_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
        var locationId = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
        await ExecuteInDbAsync(db => DbTestData.CreateDepartmentLocationAsync(db, departmentId, locationId, cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"api/Departments/{departmentId}/locations/{locationId}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
