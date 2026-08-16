using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentTests : DirectoryBaseTests
{
    public UpdateDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Update_department_With_valid_data_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var request = new UpdateDepartmentRequest("Новое название");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Departments/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal("Новое название", envelope?.Result?.Name);
        }

    [Theory]
        [InlineData("")]
        [InlineData("a")]
        public async Task Update_department_With_invalid_name_Should_return_validation_error(string name)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var request = new UpdateDepartmentRequest(name);
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Departments/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "name.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_department_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new UpdateDepartmentRequest("Новое название");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Departments/{Guid.NewGuid()}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_department_With_duplicate_name_Should_return_conflict()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Первый отдел", "first-department", null, cancellationToken));
            var secondId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Второй отдел", "second-department", null, cancellationToken));
            var request = new UpdateDepartmentRequest("Первый отдел");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Departments/{secondId}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.is.conflict", StringComparison.Ordinal));
        }
}
