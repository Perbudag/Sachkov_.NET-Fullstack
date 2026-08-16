using DirectoryService.Contracts.Locations;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class GetAllLocationsDapperTests : DirectoryBaseTests
{
    public GetAllLocationsDapperTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_all_locations_dapper_With_valid_query_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
            var url = "api/Locations/dapper?page=1&pageSize=10&sortBy=Name&sortDir=asc";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<LocationListItemDto[]>>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Single(envelope!.Result!.Value);
        }

    [Fact]
        public async Task Get_all_locations_dapper_With_invalid_query_Should_return_validation_error()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var url = "api/Locations/dapper?page=0";
    
            // act
            var response = await HttpClient.GetAsync(url, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<LocationListItemDto[]>>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.NotEmpty(envelope!.Errors!);
        }
}
