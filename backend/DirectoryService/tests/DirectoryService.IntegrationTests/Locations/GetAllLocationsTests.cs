using DirectoryService.Contracts.Locations;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class GetAllLocationsTests : DirectoryBaseTests
{
    public GetAllLocationsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_all_locations_With_valid_query_Should_return_page()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
            var url = "api/Locations?page=1&pageSize=10&sortBy=Name&sortDir=desc&search=Кремль";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<LocationListItemDto[]>>>(cancellationToken);
    
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
        [InlineData("minDepartmentCount=-1")]
        [InlineData("sortDir=invalid")]
        [InlineData("sortBy=Unknown")]
        public async Task Get_all_locations_With_invalid_query_Should_return_validation_error(string query)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var url = $"api/Locations?{query}";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<LocationListItemDto[]>>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.NotEmpty(envelope!.Errors!);
        }
}
