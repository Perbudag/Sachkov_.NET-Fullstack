using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentChildrenTests : DirectoryBaseTests
{
    public GetDepartmentChildrenTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_department_children_With_existing_children_Should_return_children()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var parentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Компания",
            "company",
            null,
            cancellationToken));
        var childId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Продажи",
            "sales",
            parentId,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{parentId}/children?page=1&pageSize=10&sortBy=Name&sortDir=asc",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.NotNull(envelope?.Result);
        Assert.Equal(1, envelope.Result.TotalCount);
        var child = Assert.Single(envelope.Result.Value);
        Assert.Equal(childId, child.Id);
        Assert.True(child.HasChildren == false);
    }

    [Fact]
    public async Task Get_department_children_With_empty_id_Should_return_validation_error()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = Guid.Empty;

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{id}/children?page=1&pageSize=10&sortBy=Id&sortDir=asc",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("page=0")]
    [InlineData("pageSize=51")]
    public async Task Get_department_children_With_invalid_paging_Should_return_validation_error(string query)
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var parentId = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Компания",
            "company",
            null,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync(
            $"api/Departments/{parentId}/children?{query}",
            cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.Contains(envelope!.Errors!, e => string.Equals(e.Code, "departments.validation.error", StringComparison.Ordinal));
    }
}
