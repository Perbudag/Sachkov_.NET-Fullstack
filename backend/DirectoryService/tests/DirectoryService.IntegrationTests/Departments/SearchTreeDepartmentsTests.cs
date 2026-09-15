using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class SearchTreeDepartmentsTests : DirectoryBaseTests
{
    public SearchTreeDepartmentsTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Search_tree_departments_With_matching_department_Should_return_department_and_ancestors()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var rootId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Компания",
            "company",
            null,
            cancellationToken));
        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Продажи",
            "sales",
            rootId,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync("api/Departments/tree/search?q=Прод", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<SearchTreeDepartmentsResultDto[]>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.NotNull(envelope?.Result);
        var result = Assert.Single(envelope.Result);
        Assert.Equal(childId, result.Node.Id);
        Assert.Contains(result.Ancestors, ancestor => ancestor.Id == rootId);
    }

    [Fact]
    public async Task Search_tree_departments_With_invalid_query_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        // act
        var response = await HttpClient.GetAsync($"api/Departments/tree/search?q=a", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<SearchTreeDepartmentsResultDto[]>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
    }
}
