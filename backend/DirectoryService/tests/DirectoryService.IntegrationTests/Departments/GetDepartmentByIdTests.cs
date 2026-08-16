using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentByIdTests : DirectoryBaseTests
{
    public GetDepartmentByIdTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_department_by_id_With_existing_id_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
    
            // act
            var response = await HttpClient.GetAsync($"api/Departments/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal("Администрация", envelope?.Result?.Name);
            Assert.Equal("administration", envelope?.Result?.Slug);
        }

    [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task Get_department_by_id_With_invalid_id_Should_return_validation_error(string id)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var url = $"api/Departments/{id}";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Get_department_by_id_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = Guid.NewGuid();
    
            // act
            var response = await HttpClient.GetAsync($"api/Departments/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<DepartmentDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found", StringComparison.Ordinal));
        }
}
