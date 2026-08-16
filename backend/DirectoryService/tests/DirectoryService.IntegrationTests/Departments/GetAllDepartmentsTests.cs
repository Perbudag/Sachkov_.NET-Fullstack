using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class GetAllDepartmentsTests : DirectoryBaseTests
{
    public GetAllDepartmentsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_all_departments_With_valid_data_Should_return_page()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(db, "Администрация", "administration", null, cancellationToken));
            var url = "api/Departments?page=1&pageSize=10&sortBy=Name&sortDir=desc&search=Администрация";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentListItemDto[]>>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.NotNull(envelope?.Result);
            Assert.Single(envelope.Result.Value);
            Assert.Equal(1, envelope.Result.TotalCount);
        }

    [Theory]
        [InlineData("page=0")]
        [InlineData("pageSize=0")]
        [InlineData("pageSize=51")]
        [InlineData("sortDir=invalid")]
        [InlineData("sortBy=Unknown")]
        public async Task Get_all_departments_With_invalid_query_Should_return_validation_error(string query)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var url = $"api/Departments?{query}";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentListItemDto[]>>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.NotEmpty(envelope!.Errors!);
        }
}
