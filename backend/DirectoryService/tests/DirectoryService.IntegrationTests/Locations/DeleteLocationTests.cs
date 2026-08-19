using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Locations;

public class DeleteLocationTests : DirectoryBaseTests
{
    public DeleteLocationTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Delete_location_With_existing_id_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = await ExecuteInDbAsync(db => DbTestData.CreateLocationAsync(db, "Московский Кремль", cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"api/Locations/{id}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var getResponse = await HttpClient.GetAsync($"api/Locations/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var locationIsDeleted = await ExecuteInDbAsync(db =>
            db.Locations
            .IgnoreQueryFilters()
            .Where(l => l.Id == id)
            .Select(l => l.IsDeleted)
            .FirstAsync(cancellationToken));

        Assert.True(locationIsDeleted);

        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        var location = await ExecuteInDbAsync(db =>
            db.Locations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken));

        Assert.Null(location);
    }

    [Fact]
    public async Task Delete_location_With_unknown_id_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.NewGuid();

        // act
        var response = await HttpClient.DeleteAsync($"api/Locations/{id}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "locations.not.found", StringComparison.Ordinal));
    }
}
