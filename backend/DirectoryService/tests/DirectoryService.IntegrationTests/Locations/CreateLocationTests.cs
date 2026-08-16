using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.Infrastructure.Postgres;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class CreateLocationTests : DirectoryBaseTests
{


    public CreateLocationTests(DirectoryTestWebFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Create_location_With_valid_data_Should_succeed(bool withApartment)
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var requestData = new CreateLocationRequest(
            Name: "Московский Кремль",
            Address: new AddressDto(
                PostalCode: "103132",
                Country: "Россия",
                Region: "Москва",
                City: "Москва",
                Street: "Кремлевская набережная",
                House: "1",
                Apartment: withApartment ? "1" : null
            )
        );

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Locations", requestData, cancellationToken);

        var responseContent = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);


        var location = await ExecuteInDbAsync(dbContext =>
        {
            return dbContext.Locations
            .Where(l => l.Id == responseContent!.Result)
            .Select(d => new LocationDto(
                Id: d.Id,
                Name: d.Name.ToString(),
                Address: new AddressDto(
                    PostalCode: d.Address.PostalCode,
                    Country: d.Address.Country,
                    Region: d.Address.Region,
                    City: d.Address.City,
                    Street: d.Address.Street,
                    House: d.Address.House,
                    Apartment: d.Address.Apartment)
            ))
            .FirstAsync(cancellationToken);
        });

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(responseContent?.IsError);
        Assert.NotNull(responseContent?.Result);
        Assert.NotNull(location);
        Assert.Equal(location.Name, requestData.Name);
        Assert.Equal(location.Address, requestData.Address);
    }
    [Theory]
        [InlineData("")]
        [InlineData("a")]
        public async Task Create_location_With_invalid_name_Should_return_validation_error(string name)
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = ApiTestData.LocationRequest(name);
    
            // act
            var response = await HttpClient.PostAsJsonAsync("api/Locations", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "name.validation.error", StringComparison.Ordinal));
        }

    [Fact]
        public async Task Create_location_With_invalid_address_Should_return_validation_error()
        {
            // arrange
            var cancellationToken = TestContext.Current.CancellationToken;
            var request = new CreateLocationRequest(
                "Московский Кремль",
                new AddressDto("12", "Россия", "Москва", "Москва", "Кремлевская набережная", "1", null));
    
            // act
            var response = await HttpClient.PostAsJsonAsync("api/Locations", request, cancellationToken);
            var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);
    
            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(envelope?.IsError);
            Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "address.validation.error", StringComparison.Ordinal));
        }

}
