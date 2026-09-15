using DirectoryService.Contracts.Departments;
using DirectoryService.IntegrationTests.TestData;
using Shared;
using System.Net;
using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Departments;

public class GetDepartmentsTreeTests : DirectoryBaseTests
{
    public GetDepartmentsTreeTests(DirectoryTestWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_departments_tree_With_existing_root_Should_return_root()
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var id = await ExecuteInDbAsync(db => DbTestData.CreateDepartmentAsync(
            db,
            "Администрация",
            "administration",
            null,
            cancellationToken));

        // act
        var response = await HttpClient.GetAsync("api/Departments/tree?page=1&pageSize=10&sortBy=Name&sortDir=asc", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(envelope?.IsError);
        Assert.NotNull(envelope?.Result);
        Assert.Contains(envelope.Result.Value, department => department.Id == id);
    }

    [Theory]
    [InlineData("page=0")]
    [InlineData("pageSize=51")]
    [InlineData("sortDir=invalid")]
    [InlineData("sortBy=Unknown")]
    public async Task Get_departments_tree_With_invalid_query_Should_return_validation_error(string query)
    {
        // arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        // act
        var response = await HttpClient.GetAsync($"api/Departments/tree?{query}", cancellationToken);
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<PageResult<DepartmentTreeNodeDto[]>>>(cancellationToken);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(envelope?.IsError);
        Assert.NotEmpty(envelope!.Errors!);
    }
}
