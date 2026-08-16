using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class UpdateLocationTests : DirectoryBaseTests
{
    public UpdateLocationTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
        public async Task Update_location_With_valid_data_Should_succeed()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
            var request = new UpdateLocationRequest("Новое место", ApiTestData.Address("Новая улица", "2"));
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Locations/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.False(envelope?.IsError);
            Assert.Equal(id, envelope?.Result?.Id);
            Assert.Equal(request.Name, envelope?.Result?.Name);
            Assert.Equal(request.Address, envelope?.Result?.Address);
        }

    [Fact]
        public async Task Update_location_With_invalid_address_Should_return_validation_error()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var id = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));
            var invalidAddress = new AddressDto("12", "Россия", "Москва", "Москва", "Кремлевская", "1", null);
            var request = new UpdateLocationRequest("Новое место", invalidAddress);
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Locations/{id}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "address.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_location_With_unknown_id_Should_return_not_found()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new UpdateLocationRequest("Новое место", ApiTestData.Address());
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Locations/{Guid.NewGuid()}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "locations.not.found", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Update_location_With_duplicate_name_Should_return_conflict()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Первое место", cancellationToken));
            var secondId = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Второе место", cancellationToken));
            var request = new UpdateLocationRequest("Первое место", null);
    
            // act
            var response = await HttpClient.PatchAsJsonAsync($"api/Locations/{secondId}", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<LocationDto?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "locations.is.conflict", StringComparison.Ordinal));
        }
}
