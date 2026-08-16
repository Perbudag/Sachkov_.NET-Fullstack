using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Positions;

public class GetAllPositionsTests : DirectoryBaseTests
{
    public GetAllPositionsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_all_positions_With_existing_data_Should_return_positions()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
    
            // act
            var response = await HttpClient.GetAsync("api/Positions", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto[]>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Contains(envelope!.Result!, position => position.Id == id);
        }
}
