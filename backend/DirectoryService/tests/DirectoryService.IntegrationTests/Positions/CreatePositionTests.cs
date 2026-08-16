using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Positions;

public class CreatePositionTests : DirectoryBaseTests
{
    public CreatePositionTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Create_position_With_valid_data_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new CreatePositionRequest("Директор");
    
            // act
            var response = await HttpClient.PostAsJsonAsync("api/Positions", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.NotNull(envelope?.Result);
        }

    [Theory]
        [InlineData("")]
        [InlineData("a")]
        public async Task Create_position_With_invalid_name_Should_return_validation_error(string name)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new CreatePositionRequest(name);
    
            // act
            var response = await HttpClient.PostAsJsonAsync("api/Positions", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "name.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Create_position_With_duplicate_name_Should_return_conflict()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new CreatePositionRequest("Директор");
            await HttpClient.PostAsJsonAsync("api/Positions", request, cancellationToken);
    
            // act
            var response = await HttpClient.PostAsJsonAsync("api/Positions", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.is.conflict", StringComparison.Ordinal));
        }
}
