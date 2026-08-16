using DirectoryService.Contracts.Departments;
using DirectoryService.Contracts.Locations;
using DirectoryService.Contracts.Positions;
using DirectoryService.Contracts.SharedDto;
using DirectoryService.IntegrationTests.TestData;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : DirectoryBaseTests
{
    public CreateDepartmentTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Create_department_With_valid_data_Should_succeed()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = ApiTestData.DepartmentRequest();

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Departments", request, cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);

        var slug = await ExecuteInDbAsync(dbContext =>
        {
            return dbContext.Departments
            .Where(d => d.Id == envelope!.Result)
            .Select(d => d.Slug.ToString())
            .FirstAsync(cancellationToken);
        });

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(envelope?.Result);
        Assert.False(envelope?.IsError);
        Assert.Equal(request.Slug, slug);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    public async Task Create_department_With_invalid_name_Should_return_validation_error(string name)
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = ApiTestData.DepartmentRequest(name: name);

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Departments", request, cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "name.validation.error", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("Administration")]
    [InlineData("-administration")]
    [InlineData("administration-")]
    public async Task Create_department_With_invalid_slug_Should_return_validation_error(string slug)
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = ApiTestData.DepartmentRequest(slug: slug);

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Departments", request, cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "slug.validation.error", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Create_department_With_non_existing_parent_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = ApiTestData.DepartmentRequest(parentId: Guid.NewGuid());

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Departments", request, cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found.parent", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Create_department_With_non_existing_location_Should_return_not_found()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var request = ApiTestData.DepartmentRequest(locationIds: [Guid.NewGuid()]);

        // act
        var response = await HttpClient.PostAsJsonAsync("api/Departments", request, cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<Guid?>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.not.found.locations", StringComparison.Ordinal));
    }
}
