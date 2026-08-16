using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class GetLocationByIdTests : DirectoryBaseTests
{
    public GetLocationByIdTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Get_location_by_id_With_existing_id_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
    
            // act
            var response = await HttpClient.GetAsync($"api/Locations/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal("Московский Кремль", envelope?.Result?.Name);
        }

    [Fact]
        public async Task Get_location_by_id_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = Guid.NewGuid();
    
            // act
            var response = await HttpClient.GetAsync($"api/Locations/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "locations.not.found", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Get_location_by_id_With_empty_id_Should_return_validation_error()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = Guid.Empty;
    
            // act
            var response = await HttpClient.GetAsync($"api/Locations/{id}", cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "locations.validation.error", StringComparison.Ordinal));
        }
}
