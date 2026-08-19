using DirectoryService.Contracts.Positions;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Positions;

public class DeletePositionTests : DirectoryBaseTests
{
    public DeletePositionTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Delete_position_With_existing_id_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = await ExecuteInDbAsync(db => DbTestData.CreatePositionAsync(db, "Директор", cancellationToken));

        // act
        var response = await HttpClient.DeleteAsync($"api/Positions/{id}", cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var getResponse = await HttpClient.GetAsync($"api/Positions/{id}", cancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        var positionIsDeleted = await ExecuteInDbAsync(db =>
            db.Positions
            .IgnoreQueryFilters()
            .Where(p => p.Id == id)
            .Select(p => p.IsDeleted)
            .FirstAsync(cancellationToken));

        Assert.True(positionIsDeleted);

        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        var position = await ExecuteInDbAsync(db =>
            db.Positions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken));

        Assert.Null(position);
    }

    [Fact]
    public async Task Delete_position_With_empty_id_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.Empty;

        // act
        var response = await HttpClient.DeleteAsync($"api/Positions/{id}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.validation.error", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Delete_position_With_unknown_id_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.NewGuid();

        // act
        var response = await HttpClient.DeleteAsync($"api/Positions/{id}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<object?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "positions.not.found", StringComparison.Ordinal));
    }
}
