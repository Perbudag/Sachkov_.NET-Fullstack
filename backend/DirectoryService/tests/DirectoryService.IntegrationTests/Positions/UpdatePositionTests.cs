using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Positions;

public class UpdatePositionTests : DirectoryBaseTests
{
    public UpdatePositionTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Update_position_With_valid_data_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
            var request = new UpdatePositionRequest("Главный директор");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Positions/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal(request.Name, envelope?.Result?.Name);
        }

    [Theory]
        [InlineData("")]
        [InlineData("a")]
        public async Task Update_position_With_invalid_name_Should_return_validation_error(string name)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));
            var request = new UpdatePositionRequest(name);
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Positions/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "name.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_position_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new UpdatePositionRequest("Главный директор");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Positions/{Guid.NewGuid()}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.not.found", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_position_With_duplicate_name_Should_return_conflict()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Первый директор", cancellationToken));
            var secondId = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Второй директор", cancellationToken));
            var request = new UpdatePositionRequest("Первый директор");
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Positions/{secondId}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<PositionDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.is.conflict", StringComparison.Ordinal));
        }
}
