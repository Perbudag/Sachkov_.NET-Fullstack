using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class RemovePositionFromDepartmentTests : DirectoryBaseTests
{
    public RemovePositionFromDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Remove_position_from_department_When_relation_missing_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var positionId = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
    
            // act
            var response = await HttpClient.DeleteAsync($"departments/{departmentId}/positions/{positionId}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found.position", StringComparison.Ordinal));
        }

    [Fact]
    public async Task Remove_position_from_department_With_existing_relation_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
        var positionId = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
        await ExecuteInDbAsync(db => DbTestData.CreateDepartmentPositionAsync(db, departmentId, positionId, cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"departments/{departmentId}/positions/{positionId}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
