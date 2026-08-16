using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class AddPositionToDepartmentTests : DirectoryBaseTests
{
    public AddPositionToDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Add_position_to_department_With_valid_data_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var positionId = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
    
            // act
            var response = await HttpClient.PostAsync($"api/Departments/{departmentId}/positions/{positionId}", null, cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

    [Fact]
        public async Task Add_position_to_department_With_duplicate_relation_Should_return_conflict()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var positionId = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
            await ExecuteInDbAsync(db => DbTestData.CreateDepartmentPositionAsync(db, departmentId, positionId, cancellationToken));
    
            // act
            var response = await HttpClient.PostAsync($"api/Departments/{departmentId}/positions/{positionId}", null, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<bool?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.is.conflict.position", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Add_position_to_department_With_unknown_position_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var departmentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
    
            // act
            var response = await HttpClient.PostAsync($"api/Departments/{departmentId}/positions/{Guid.NewGuid()}", null, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found.position", StringComparison.Ordinal));
        }
}
