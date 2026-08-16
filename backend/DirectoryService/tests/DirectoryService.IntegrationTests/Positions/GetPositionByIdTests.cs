using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Positions;

public class GetPositionByIdTests : DirectoryBaseTests
{
    public GetPositionByIdTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_position_by_id_With_existing_id_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
    
            // act
            var response = await HttpClient.GetAsync($"api/Positions/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal("Директор", envelope?.Result?.Name);
        }

    [Fact]
        public async Task Get_position_by_id_With_empty_id_Should_return_validation_error()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = Guid.Empty;
    
            // act
            var response = await HttpClient.GetAsync($"api/Positions/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Get_position_by_id_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = Guid.NewGuid();
    
            // act
            var response = await HttpClient.GetAsync($"api/Positions/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.not.found", StringComparison.Ordinal));
        }
}
